using DataTransferAndIntegrationSystem.Application.DTOs;
namespace DataTransferAndIntegrationSystem.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(
        LoginRequestDto request);
}