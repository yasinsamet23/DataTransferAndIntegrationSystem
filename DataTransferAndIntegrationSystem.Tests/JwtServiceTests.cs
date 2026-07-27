using System.IdentityModel.Tokens.Jwt;
using DataTransferAndIntegrationSystem.Application.Interfaces;
using DataTransferAndIntegrationSystem.Infrastructure.Services;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;
using System.Security.Claims;

namespace DataTransferAndIntegrationSystem.Tests.Services;

public class JwtServiceTests
{
    private readonly Mock<IJwtSettings> _jwtSettingsMock;
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        _jwtSettingsMock = new Mock<IJwtSettings>();

        _jwtSettingsMock.Setup(x => x.Key).Returns("ThisIsMyVerySecretJwtKey123456789");
        _jwtSettingsMock.Setup(x => x.Issuer).Returns("DataTransferAndIntegrationSystem");
        _jwtSettingsMock.Setup(x => x.Audience).Returns("DataTransferAndIntegrationSystemClient");
        _jwtSettingsMock.Setup(x => x.ExpirationMinutes).Returns(60);

        _jwtService = new JwtService(_jwtSettingsMock.Object);
    }

    #region Helper Methods

    private JwtSecurityToken ParseToken(string tokenString)
    {
        var handler = new JwtSecurityTokenHandler();
        return handler.ReadJwtToken(tokenString);
    }

    #endregion

    #region Token Generation & Validation Tests

    [Fact]
    public void GenerateToken_ShouldReturnToken()
    {
        // Arrange
        var username = "admin";
        var role = "Admin";

        // Act
        var result = _jwtService.GenerateToken(username, role);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateToken_ShouldGenerateValidJwtToken()
    {
        // Arrange & Act
        var result = _jwtService.GenerateToken("user", "User");
        var handler = new JwtSecurityTokenHandler();

        // Assert
        handler.CanReadToken(result.Token).Should().BeTrue();
    }

    [Fact]
    public void GenerateToken_ShouldSetCorrectExpiration()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var result = _jwtService.GenerateToken("admin", "Admin");
        var after = DateTime.UtcNow;

        // Assert
        result.Expiration.Should().BeOnOrAfter(before.AddMinutes(60));
        result.Expiration.Should().BeOnOrBefore(after.AddMinutes(60));
    }

    #endregion

    #region Token Claims Validation Tests

    [Fact]
    public void GenerateToken_ShouldContainCorrectUsernameClaim()
    {
        // Arrange & Act
        var result = _jwtService.GenerateToken("admin", "Admin");
        var token = ParseToken(result.Token);

        // Assert
        token.Claims.First(x => x.Type == ClaimTypes.Name).Value.Should().Be("admin");
    }

    [Fact]
    public void GenerateToken_ShouldContainCorrectRoleClaim()
    {
        // Arrange & Act
        var result = _jwtService.GenerateToken("admin", "Admin");
        var token = ParseToken(result.Token);

        // Assert
        token.Claims.First(x => x.Type == ClaimTypes.Role).Value.Should().Be("Admin");
    }

    [Fact]
    public void GenerateToken_ShouldContainCorrectIssuer()
    {
        // Arrange & Act
        var result = _jwtService.GenerateToken("admin", "Admin");
        var token = ParseToken(result.Token);

        // Assert
        token.Issuer.Should().Be("DataTransferAndIntegrationSystem");
    }

    [Fact]
    public void GenerateToken_ShouldContainCorrectAudience()
    {
        // Arrange & Act
        var result = _jwtService.GenerateToken("admin", "Admin");
        var token = ParseToken(result.Token);

        // Assert
        token.Audiences.Should().Contain("DataTransferAndIntegrationSystemClient");
    }

    #endregion
}