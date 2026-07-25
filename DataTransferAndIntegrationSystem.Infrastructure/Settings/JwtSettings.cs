using DataTransferAndIntegrationSystem.Application.Interfaces;

namespace DataTransferAndIntegrationSystem.Infrastructure.Settings;

public class JwtSettings : IJwtSettings
{
    public string Key { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public int ExpirationMinutes { get; set; }
}