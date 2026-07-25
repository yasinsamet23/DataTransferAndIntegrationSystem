namespace DataTransferAndIntegrationSystem.Application.Interfaces;

public interface IJwtSettings
{
    string Key { get; }

    string Issuer { get; }

    string Audience { get; }

    int ExpirationMinutes { get; }
}