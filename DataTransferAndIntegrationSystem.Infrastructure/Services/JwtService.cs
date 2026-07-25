using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DataTransferAndIntegrationSystem.Application.DTOs;
using DataTransferAndIntegrationSystem.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace DataTransferAndIntegrationSystem.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IJwtSettings _jwtSettings;

    public JwtService(IJwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    public LoginResponseDto GenerateToken(
        string username,
        string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var expiration =
            DateTime.UtcNow.AddMinutes(
                _jwtSettings.ExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        return new LoginResponseDto
        {
            Token = new JwtSecurityTokenHandler()
                .WriteToken(token),

            Expiration = expiration
        };
    }
}