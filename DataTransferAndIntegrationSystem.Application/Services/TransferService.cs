using DataTransferAndIntegrationSystem.Application.DTOs;
using DataTransferAndIntegrationSystem.Application.Interfaces;

namespace DataTransferAndIntegrationSystem.Application.Services;

public class TransferService : ITransferService
{
    private readonly IUserRepository _userRepository;

    public TransferService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<TransferResultDto> StartTransferAsync()
    {
        // Şimdilik örnek veri oluşturuyoruz.
        // Daha sonra bunu Mock REST API'den okuyacağız.

        var externalUsers = new List<ExternalUserDto>
        {
            new ExternalUserDto
            {
                Name = "Ali",
                Email = "ali@test.com",
                Phone = "5551111111"
            },
            new ExternalUserDto
            {
                Name = "Veli",
                Email = "veli@test.com",
                Phone = "5552222222"
            }
        };

        int successCount = 0;
        int failedCount = 0;

        foreach (var externalUser in externalUsers)
        {
            var existingUser =
                await _userRepository.GetByEmailAsync(externalUser.Email);

            if (existingUser != null)
            {
                failedCount++;
                continue;
            }

            var user = new Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                Name = externalUser.Name,
                Email = externalUser.Email,
                Phone = externalUser.Phone,
                CreatedDate = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            successCount++;
        }

        await _userRepository.SaveChangesAsync();

        return new TransferResultDto
        {
            TotalRecords = externalUsers.Count,
            SuccessfulRecords = successCount,
            FailedRecords = failedCount,
            Message = "Transfer completed successfully."
        };
    }
}