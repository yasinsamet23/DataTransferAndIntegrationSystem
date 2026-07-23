using DataTransferAndIntegrationSystem.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DataTransferAndIntegrationSystem.Infrastructure.Services;

public class MockarooSettings : IMockarooSettings
{
    private readonly IConfiguration _configuration;

    public MockarooSettings(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string MockarooUrl =>
        $"{_configuration["Mockaroo:BaseUrl"]}?count={_configuration["Mockaroo:Count"]}&key={_configuration["Mockaroo:ApiKey"]}";
}