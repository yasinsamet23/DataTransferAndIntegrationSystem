using DataTransferAndIntegrationSystem.Application.DTOs;

namespace DataTransferAndIntegrationSystem.Application.Interfaces;

public interface ICsvReaderService
{
    Task<List<ExternalUserDto>> ReadUsersAsync(Stream stream);

    void ValidateHeader(Stream stream);
}