using DataTransferAndIntegrationSystem.Application.DTOs;
using DataTransferAndIntegrationSystem.Application.Interfaces;

namespace DataTransferAndIntegrationSystem.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IJwtService _jwtService;

    public AuthService(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    public Task<LoginResponseDto?> LoginAsync(
        LoginRequestDto request)
    {
        if (request.Username != "admin" ||
            request.Password != "123456")
        {
            return Task.FromResult<LoginResponseDto?>(null);
        }

        var token = _jwtService.GenerateToken(
            request.Username,
            "Admin");

        return Task.FromResult<LoginResponseDto?>(token);
    }
}