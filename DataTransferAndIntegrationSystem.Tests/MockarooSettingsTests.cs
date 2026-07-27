using DataTransferAndIntegrationSystem.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace DataTransferAndIntegrationSystem.Tests.Services;

public class MockarooSettingsTests
{
    #region Helper Methods

    private MockarooSettings CreateSettings(string baseUrl, string count, string apiKey)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Mockaroo:BaseUrl", baseUrl },
                { "Mockaroo:Count", count },
                { "Mockaroo:ApiKey", apiKey }
            })
            .Build();

        return new MockarooSettings(configuration);
    }

    #endregion

    #region Tests

    [Fact]
    public void MockarooUrl_ShouldBuildCorrectUrl()
    {
        // Arrange
        var settings = CreateSettings(
            baseUrl: "https://api.mockaroo.com/api/users.json", 
            count: "100", 
            apiKey: "abc123");

        // Act
        var result = settings.MockarooUrl;

        // Assert
        result.Should().Be("https://api.mockaroo.com/api/users.json?count=100&key=abc123");
    }

    [Fact]
    public void MockarooUrl_ShouldContainConfiguredCount()
    {
        // Arrange
        var settings = CreateSettings(
            baseUrl: "https://test.com/users", 
            count: "500", 
            apiKey: "key123");

        // Act
        var result = settings.MockarooUrl;

        // Assert
        result.Should().Contain("count=500");
    }

    [Fact]
    public void MockarooUrl_ShouldContainApiKey()
    {
        // Arrange
        var settings = CreateSettings(
            baseUrl: "https://test.com/users", 
            count: "50", 
            apiKey: "secretKey");

        // Act
        var result = settings.MockarooUrl;

        // Assert
        result.Should().Contain("key=secretKey");
    }

    #endregion
}