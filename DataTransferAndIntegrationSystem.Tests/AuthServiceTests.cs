using DataTransferAndIntegrationSystem.Application.DTOs;
using DataTransferAndIntegrationSystem.Application.Interfaces;
using DataTransferAndIntegrationSystem.Infrastructure.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace DataTransferAndIntegrationSystem.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _jwtServiceMock = new Mock<IJwtService>();
        _authService = new AuthService(_jwtServiceMock.Object);
    }

    #region Helper Methods

    private LoginRequestDto CreateLoginRequest(string username, string password)
    {
        return new LoginRequestDto
        {
            Username = username,
            Password = password
        };
    }

    #endregion

    #region Success Tests

    [Fact]
    public async Task LoginAsync_AdminCredentials_ShouldReturnToken()
    {
        // Arrange
        var request = CreateLoginRequest("admin", "123456");

        var expectedResponse = new LoginResponseDto
        {
            Token = "admin-token",
            Expiration = DateTime.UtcNow.AddMinutes(30)
        };

        _jwtServiceMock
            .Setup(x => x.GenerateToken("admin", "Admin"))
            .Returns(expectedResponse);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Token.Should().Be("admin-token");

        _jwtServiceMock.Verify(x => x.GenerateToken("admin", "Admin"), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_UserCredentials_ShouldReturnToken()
    {
        // Arrange
        var request = CreateLoginRequest("user", "123456");

        var expectedResponse = new LoginResponseDto
        {
            Token = "user-token",
            Expiration = DateTime.UtcNow.AddMinutes(30)
        };

        _jwtServiceMock
            .Setup(x => x.GenerateToken("user", "User"))
            .Returns(expectedResponse);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Token.Should().Be("user-token");

        _jwtServiceMock.Verify(x => x.GenerateToken("user", "User"), Times.Once);
    }

    #endregion

    #region Failure Tests

    [Fact]
    public async Task LoginAsync_InvalidUsername_ShouldReturnNull()
    {
        // Arrange
        var request = CreateLoginRequest("manager", "123456");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().BeNull();

        _jwtServiceMock.Verify(
            x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>()), 
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ShouldReturnNull()
    {
        // Arrange
        var request = CreateLoginRequest("admin", "654321");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().BeNull();

        _jwtServiceMock.Verify(
            x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>()), 
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_InvalidUsernameAndPassword_ShouldReturnNull()
    {
        // Arrange
        var request = CreateLoginRequest("test", "test");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().BeNull();

        _jwtServiceMock.Verify(
            x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>()), 
            Times.Never);
    }

    #endregion
}