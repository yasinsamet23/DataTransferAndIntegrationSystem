using DataTransferAndIntegrationSystem.Application.DTOs;

namespace DataTransferAndIntegrationSystem.Application.Interfaces;

public interface IJwtService
{
    LoginResponseDto GenerateToken(
        string username,
        string role);
}