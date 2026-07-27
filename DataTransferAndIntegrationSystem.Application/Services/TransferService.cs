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
    private readonly IAnomalyDetectionService _anomalyDetectionService;

    private const string DummyJsonUrl =
    "https://dummyjson.com/users";


    public TransferService(
    IUserRepository userRepository,
    HttpClient httpClient,
    ITransferLogService transferLogService,
    IErrorLogService errorLogService,
    IMockarooSettings mockarooSettings,
    IAnomalyDetectionService anomalyDetectionService)
    {
        _userRepository = userRepository;
        _httpClient = httpClient;
        _transferLogService = transferLogService;
        _errorLogService = errorLogService;
        _mockarooSettings = mockarooSettings;
        _anomalyDetectionService = anomalyDetectionService;
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

    private static void AddError(
    List<ErrorLogDto> errorsToInsert,
    Guid transferLogId,
    Guid recordId,
    string field,
    string message)
    {
        errorsToInsert.Add(new ErrorLogDto
        {
            Id = Guid.NewGuid(),
            TransferLogId = transferLogId,
            RecordId = recordId,
            ErrorField = field,
            ErrorMessage = message,
            CreatedDate = DateTime.UtcNow
        });
    }

    private async Task<bool> ValidateUserAsync(
    ExternalUserDto externalUser,
    Guid transferLogId,
    HashSet<string> processedEmails,
    List<ErrorLogDto> errorsToInsert)
    {
        // FirstName kontrolü
        if (string.IsNullOrWhiteSpace(externalUser.FirstName))
        {
            AddError(
                errorsToInsert,
                transferLogId,
                Guid.NewGuid(),
                "FirstName",
                "First name is required.");

            return false;
        }

        // Email boş mu?
        if (string.IsNullOrWhiteSpace(externalUser.Email))
        {
            AddError(
                errorsToInsert,
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
            AddError(
                errorsToInsert,
                transferLogId,
                Guid.NewGuid(),
                "Email",
                "Invalid email format.");

            return false;
        }

        // Aynı transfer paketinde duplicate email var mı?
        if (processedEmails.Contains(externalUser.Email))
        {
            AddError(
                errorsToInsert,
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
            AddError(
                errorsToInsert,
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


    private async Task<TransferResultDto> ExecuteTransferAsync(
    List<ExternalUserDto> users)
    {
        int successCount = 0;
        int failedCount = 0;
        var usersToInsert = new List<User>();
        var errorsToInsert = new List<ErrorLogDto>();
        var transferLogId = Guid.NewGuid();

        var processedEmails = new HashSet<string>();

        foreach (var externalUser in users)
        {


            if (!await ValidateUserAsync(
                externalUser,
                transferLogId,
                processedEmails,
                errorsToInsert))
            {
                failedCount++;
                continue;
            }
            
            var anomalyResult =
    _anomalyDetectionService.ValidateUser(externalUser);

            if (!anomalyResult.IsValid)
            {
                var recordId = Guid.NewGuid();

                foreach (var error in anomalyResult.Errors)
                {
                    AddError(
                        errorsToInsert,
                        transferLogId,
                        recordId,
                        error.Field,
                        error.Message);
                }

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

        await _transferLogService.BulkInsertTransferLogsAsync(
            new List<TransferLogDto>
            {
                new()
                {
                    Id = transferLogId,
                    TransferDate = DateTime.UtcNow,
                    TotalRecords = users.Count,
                    SuccessCount = successCount,
                    Status = result.Status
                }
            });

        if (errorsToInsert.Count > 0)
        {
            await _errorLogService.BulkInsertErrorsAsync(errorsToInsert);
        }

        return new TransferResultDto
        {
            TotalRecords = users.Count,
            SuccessfulRecords = successCount,
            FailedRecords = failedCount,
            Message = result.Message
        };
    }




}
