using System.Text.Json;
using DataTransferAndIntegrationSystem.Application.DTOs;
using DataTransferAndIntegrationSystem.Application.Interfaces;
using DataTransferAndIntegrationSystem.Domain.Entities;
using System.Text.RegularExpressions;

namespace DataTransferAndIntegrationSystem.Application.Services;

public class TransferService : ITransferService
{
    private readonly IUserRepository _userRepository;
    private readonly HttpClient _httpClient;
    private readonly ITransferLogService _transferLogService;
    private readonly IErrorLogService _errorLogService;
    private readonly IMockarooSettings _mockarooSettings;

    private const string DummyJsonUrl =
    "https://dummyjson.com/users";


    public TransferService(
    IUserRepository userRepository,
    HttpClient httpClient,
    ITransferLogService transferLogService,
    IErrorLogService errorLogService,
    IMockarooSettings mockarooSettings)
    {
        _userRepository = userRepository;
        _httpClient = httpClient;
        _transferLogService = transferLogService;
        _errorLogService = errorLogService;
        _mockarooSettings = mockarooSettings;

    }

    public async Task<TransferResultDto> StartTransferAsync()
    {
        var externalUsers =
       await GetDummyUsersAsync();

        return await ExecuteTransferAsync(
            externalUsers.Users);


    }

    public async Task<TransferResultDto> StartNightlyTransferAsync()
    {
        var users = await GetMockarooUsersAsync();

        return await ExecuteTransferAsync(users);
    }

    public async Task<TransferResultDto> StartCsvTransferAsync(
    List<ExternalUserDto> users)
    {
        return await ExecuteTransferAsync(users);
    }


    private async Task<ExternalUsersResponseDto> GetDummyUsersAsync()
    {
        var response =
            await _httpClient.GetAsync(DummyJsonUrl);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var users =
            JsonSerializer.Deserialize<ExternalUsersResponseDto>(
                json,
                options);

        if (users == null)
            throw new Exception("Users could not be retrieved from the external API.");

        return users;
    }

    private async Task<List<ExternalUserDto>> GetMockarooUsersAsync()
    {
        var response =
            await _httpClient.GetAsync(_mockarooSettings.MockarooUrl);

        response.EnsureSuccessStatusCode();

        var json =
            await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var users =
            JsonSerializer.Deserialize<List<ExternalUserDto>>(
                json,
                options);

        if (users == null)
            throw new Exception("Users could not be retrieved from Mockaroo.");

        return users;
    }

    private async Task CreateRunningTransferLogAsync(Guid transferLogId)
    {
        await _transferLogService.AddTransferLogAsync(
            new TransferLogDto
            {
                Id = transferLogId,
                TransferDate = DateTime.UtcNow,
                TotalRecords = 0,
                SuccessCount = 0,
                Status = "Running"
            });
    }

    private User CreateUser(ExternalUserDto externalUser)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = $"{externalUser.FirstName} {externalUser.LastName}",
            Email = externalUser.Email,
            Phone = externalUser.Phone,
            CreatedDate = DateTime.UtcNow
        };
    }

    private async Task AddErrorAsync(
    Guid transferLogId,
    Guid recordId,
    string field,
    string message)
    {
        await _errorLogService.AddErrorAsync(
            new ErrorLogDto
            {
                TransferLogId = transferLogId,
                RecordId = recordId,
                ErrorField = field,
                ErrorMessage = message
            });
    }

    private async Task<bool> ValidateUserAsync(
    ExternalUserDto externalUser,
    Guid transferLogId,
    HashSet<string> processedEmails)
    {
        // FirstName kontrolü
        if (string.IsNullOrWhiteSpace(externalUser.FirstName))
        {
            await AddErrorAsync(
                transferLogId,
                Guid.NewGuid(),
                "FirstName",
                "First name is required.");

            return false;
        }

        // Email boş mu?
        if (string.IsNullOrWhiteSpace(externalUser.Email))
        {
            await AddErrorAsync(
                transferLogId,
                Guid.NewGuid(),
                "Email",
                "Email is required.");

            return false;
        }

        // Email formatı doğru mu?
        if (!Regex.IsMatch(
            externalUser.Email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            await AddErrorAsync(
                transferLogId,
                Guid.NewGuid(),
                "Email",
                "Invalid email format.");

            return false;
        }

        // Aynı transfer paketinde duplicate email var mı?
        if (processedEmails.Contains(externalUser.Email))
        {
            await AddErrorAsync(
                transferLogId,
                Guid.NewGuid(),
                "Email",
                "Duplicate email in transfer package.");

            return false;
        }

        // Veritabanında aynı email var mı?
        var existingUser =
            await _userRepository.GetByEmailAsync(externalUser.Email);

        if (existingUser != null)
        {
            await AddErrorAsync(
                transferLogId,
                existingUser.Id,
                "Email",
                "User already exists.");

            return false;
        }

        return true;
    }

    private (string Status, string Message)
    CalculateTransferResult(
        int successCount,
        int failedCount)
    {
        if (failedCount == 0)
            return ("Completed",
                    "Transfer completed successfully.");

        if (successCount == 0)
            return ("Failed",
                    "Transfer failed. No users were transferred.");

        return ("Completed With Errors",
                "Transfer completed with errors.");
    }


    private async Task UpdateTransferLogAsync(
    Guid transferLogId,
    int totalRecords,
    int successCount,
    string status)
    {
        await _transferLogService.UpdateTransferLogAsync(
            new TransferLogDto
            {
                Id = transferLogId,
                TransferDate = DateTime.UtcNow,
                TotalRecords = totalRecords,
                SuccessCount = successCount,
                Status = status
            });
    }

    private async Task<TransferResultDto> ExecuteTransferAsync(
    List<ExternalUserDto> users)
    {
        int successCount = 0;
        int failedCount = 0;
        var usersToInsert = new List<User>();
        var transferLogId = Guid.NewGuid();

        var processedEmails = new HashSet<string>();

        await CreateRunningTransferLogAsync(transferLogId);

        foreach (var externalUser in users)
        {


            if (!await ValidateUserAsync(
                externalUser,
                transferLogId,
                processedEmails))
            {
                failedCount++;
                continue;
            }


            var user = CreateUser(externalUser);

            processedEmails.Add(user.Email);

            usersToInsert.Add(user);

            successCount++;
        }

        if (usersToInsert.Count > 0)
        {
            await _userRepository.BulkInsertAsync(usersToInsert);
        }



        var result = CalculateTransferResult(successCount, failedCount);

        await UpdateTransferLogAsync(
        transferLogId,
        users.Count,
        successCount,
        result.Status);

        return new TransferResultDto
        {
            TotalRecords = users.Count,
            SuccessfulRecords = successCount,
            FailedRecords = failedCount,
            Message = result.Message
        };
    }




}