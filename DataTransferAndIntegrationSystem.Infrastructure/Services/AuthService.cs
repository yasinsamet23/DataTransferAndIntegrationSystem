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
        if (request.Username == "admin" &&
            request.Password == "123456")
        {
            var token = _jwtService.GenerateToken(
                "admin",
                "Admin");

            return Task.FromResult<LoginResponseDto?>(token);
        }

        if (request.Username == "user" &&
            request.Password == "123456")
        {
            var token = _jwtService.GenerateToken(
                "user",
                "User");

            return Task.FromResult<LoginResponseDto?>(token);
        }

        return Task.FromResult<LoginResponseDto?>(null);
    }
}