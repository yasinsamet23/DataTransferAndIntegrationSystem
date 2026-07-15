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
    

    public TransferService(
    IUserRepository userRepository,
    HttpClient httpClient,
    ITransferLogService transferLogService,
    IErrorLogService errorLogService)
    {
        _userRepository = userRepository;
        _httpClient = httpClient;
        _transferLogService = transferLogService;
        _errorLogService = errorLogService;
    }

    public async Task<TransferResultDto> StartTransferAsync()
    {
        var response = await _httpClient.GetAsync("https://dummyjson.com/users");

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var externalUsers =
            JsonSerializer.Deserialize<ExternalUsersResponseDto>(json, options);

        if (externalUsers == null)
        {
            throw new Exception("Users could not be retrieved from the external API.");
        }

        int successCount = 0;
        int failedCount = 0;

        var processedEmails = new HashSet<string>();
        
        foreach (var externalUser in externalUsers.Users)
        {
            
            
            if (string.IsNullOrWhiteSpace(externalUser.FirstName))
            {
                await _errorLogService.AddErrorAsync(
                    new ErrorLogDto
                    {
                        RecordId = Guid.NewGuid(),
                        ErrorField = "FirstName",
                        ErrorMessage = "First name is required."
                    });

                failedCount++;
                continue;
            }
            

            if (string.IsNullOrWhiteSpace(externalUser.Email))
            {
                await _errorLogService.AddErrorAsync(
                    new ErrorLogDto
                    {
                        RecordId = Guid.NewGuid(),
                        ErrorField = "Email",
                        ErrorMessage = "Email is required."
                    });

                failedCount++;
                continue;
            }
            
            if (!Regex.IsMatch(
    externalUser.Email,
    @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                await _errorLogService.AddErrorAsync(
                    new ErrorLogDto
                    {
                        RecordId = Guid.NewGuid(),
                        ErrorField = "Email",
                        ErrorMessage = "Invalid email format."
                    });

                failedCount++;
                continue;
            }

            if (processedEmails.Contains(externalUser.Email))
            {
                await _errorLogService.AddErrorAsync(
                    new ErrorLogDto
                    {
                        RecordId = Guid.NewGuid(),
                        ErrorField = "Email",
                        ErrorMessage = "Duplicate email in transfer package."
                    });

                failedCount++;
                continue;
            }              
            var existingUser =
                await _userRepository.GetByEmailAsync(externalUser.Email);

            if (existingUser != null)
            {
                await _errorLogService.AddErrorAsync(
                    new ErrorLogDto
                    {
                        RecordId = existingUser.Id,
                        ErrorField = "Email",
                        ErrorMessage = "User already exists."
                    });

                failedCount++;
                continue;
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = $"{externalUser.FirstName} {externalUser.LastName}",
                Email = externalUser.Email,
                Phone = externalUser.Phone,
                CreatedDate = DateTime.UtcNow
            };

            processedEmails.Add(externalUser.Email);
            
            await _userRepository.AddAsync(user);

            successCount++;
        }

        await _userRepository.SaveChangesAsync();

        string status;

        if (failedCount == 0)
        {
            status = "Completed";
        }
        else if (successCount == 0)
        {
            status = "Failed";
        }
        else
        {
            status = "Completed With Errors";
        }
        
        await _transferLogService.AddTransferLogAsync(
        new TransferLogDto
        {
            TransferDate = DateTime.UtcNow,
            TotalRecords = externalUsers.Users.Count,
            SuccessCount = successCount,
            Status = status
        });

        return new TransferResultDto
        {
            TotalRecords = externalUsers.Users.Count,
            SuccessfulRecords = successCount,
            FailedRecords = failedCount,
            Message = successCount == 0
            ? "No new users were transferred."
            : "Transfer completed successfully."
        };
    }
}