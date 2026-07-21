using System.Net;
using System.Text;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Xunit;
using DataTransferAndIntegrationSystem.Application.DTOs;
using DataTransferAndIntegrationSystem.Application.Interfaces;
using DataTransferAndIntegrationSystem.Application.Services;
using DataTransferAndIntegrationSystem.Domain.Entities;

namespace DataTransferAndIntegrationSystem.Tests.Services;

public class TransferServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITransferLogService> _transferLogServiceMock;
    private readonly Mock<IErrorLogService> _errorLogServiceMock;

    public TransferServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _transferLogServiceMock = new Mock<ITransferLogService>();
        _errorLogServiceMock = new Mock<IErrorLogService>();

        // Varsayılan davranış: Kullanıcının veritabanında bulunmadığı durum
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);
    }

    [Fact]
    public async Task StartTransferAsync_AllSuccessful_ShouldReturnCompleted()
    {
        // Arrange
        var json = @"{ ""users"": [ { ""firstName"": ""John"", ""lastName"": ""Doe"", ""email"": ""john.doe@test.com"", ""phone"": ""5551112233"" } ] }";
        var transferService = CreateServiceWithHttpResponse(json);

        // Act
        var result = await transferService.StartTransferAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalRecords.Should().Be(1);
        result.SuccessfulRecords.Should().Be(1);
        result.FailedRecords.Should().Be(0);
        result.Message.Should().Be("Transfer completed successfully.");

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        _transferLogServiceMock.Verify(x => x.AddTransferLogAsync(It.IsAny<TransferLogDto>()), Times.Once);
        _transferLogServiceMock.Verify(x => x.UpdateTransferLogAsync(It.Is<TransferLogDto>(t => t.Status == "Completed")), Times.Once);
        _errorLogServiceMock.Verify(x => x.AddErrorAsync(It.IsAny<ErrorLogDto>()), Times.Never);
    }

    [Fact]
    public async Task StartTransferAsync_PartiallySuccessful_ShouldReturnCompletedWithErrors()
    {
        // Arrange
        var json = @"{ ""users"": [ 
            { ""firstName"": ""John"", ""lastName"": ""Doe"", ""email"": ""john.doe@example.com"", ""phone"": ""+1-202-555-0101"" }, 
            { ""firstName"": ""Jane"", ""lastName"": ""Smith"", ""email"": """", ""phone"": ""+1-202-555-0102"" } 
        ] }";
        var transferService = CreateServiceWithHttpResponse(json);

        // Act
        var result = await transferService.StartTransferAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalRecords.Should().Be(2);
        result.SuccessfulRecords.Should().Be(1);
        result.FailedRecords.Should().Be(1);
        result.Message.Should().Be("Transfer completed with errors.");

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        _transferLogServiceMock.Verify(x => x.AddTransferLogAsync(It.IsAny<TransferLogDto>()), Times.Once);
        _transferLogServiceMock.Verify(x => x.UpdateTransferLogAsync(It.Is<TransferLogDto>(t => t.Status == "Completed With Errors")), Times.Once);
        _errorLogServiceMock.Verify(x => x.AddErrorAsync(It.IsAny<ErrorLogDto>()), Times.Once);
    }

    [Fact]
    public async Task StartTransferAsync_NoSuccessfulTransfer_ShouldReturnFailed()
    {
        // Arrange
        var json = @"{ ""users"": [ { ""firstName"": """", ""lastName"": ""Doe"", ""email"": ""john.doe@test.com"", ""phone"": ""5551112233"" } ] }";
        var transferService = CreateServiceWithHttpResponse(json);

        // Act
        var result = await transferService.StartTransferAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalRecords.Should().Be(1);
        result.SuccessfulRecords.Should().Be(0);
        result.FailedRecords.Should().Be(1);
        result.Message.Should().Be("Transfer failed. No users were transferred.");

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        _transferLogServiceMock.Verify(x => x.UpdateTransferLogAsync(It.Is<TransferLogDto>(t => t.Status == "Failed")), Times.Once);
        _errorLogServiceMock.Verify(x => x.AddErrorAsync(It.IsAny<ErrorLogDto>()), Times.Once);
    }

    [Fact]
    public async Task StartTransferAsync_FirstNameIsEmpty_ShouldCreateErrorLog()
    {
        // Arrange
        var json = @"{ ""users"": [ { ""firstName"": """", ""lastName"": ""Doe"", ""email"": ""john.doe@example.com"", ""phone"": ""+1-202-555-0101"" } ] }";
        var transferService = CreateServiceWithHttpResponse(json);

        // Act
        var result = await transferService.StartTransferAsync();

        // Assert
        result.TotalRecords.Should().Be(1);
        result.SuccessfulRecords.Should().Be(0);
        result.FailedRecords.Should().Be(1);
        result.Message.Should().Be("Transfer failed. No users were transferred.");

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
        _errorLogServiceMock.Verify(x => x.AddErrorAsync(It.Is<ErrorLogDto>(e => 
            e.ErrorField == "FirstName" && e.ErrorMessage == "First name is required.")), Times.Once);
    }

    [Fact]
    public async Task StartTransferAsync_EmailIsEmpty_ShouldCreateErrorLog()
    {
        // Arrange
        var json = @"{ ""users"": [ { ""firstName"": ""John"", ""lastName"": ""Doe"", ""email"": """", ""phone"": ""+1-202-555-0101"" } ] }";
        var transferService = CreateServiceWithHttpResponse(json);

        // Act
        var result = await transferService.StartTransferAsync();

        // Assert
        result.SuccessfulRecords.Should().Be(0);
        result.FailedRecords.Should().Be(1);

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
        _errorLogServiceMock.Verify(x => x.AddErrorAsync(It.Is<ErrorLogDto>(e => 
            e.ErrorField == "Email" && e.ErrorMessage == "Email is required.")), Times.Once);
    }

    [Fact]
    public async Task StartTransferAsync_InvalidEmail_ShouldCreateErrorLog()
    {
        // Arrange
        var json = @"{ ""users"": [ { ""firstName"": ""John"", ""lastName"": ""Doe"", ""email"": ""invalid-email"", ""phone"": ""+1-202-555-0101"" } ] }";
        var transferService = CreateServiceWithHttpResponse(json);

        // Act
        var result = await transferService.StartTransferAsync();

        // Assert
        result.SuccessfulRecords.Should().Be(0);
        result.FailedRecords.Should().Be(1);

        _errorLogServiceMock.Verify(x => x.AddErrorAsync(It.Is<ErrorLogDto>(e => 
            e.ErrorField == "Email" && e.ErrorMessage == "Invalid email format.")), Times.Once);
    }

    [Fact]
    public async Task StartTransferAsync_DuplicateEmail_ShouldCreateErrorLog()
    {
        // Arrange
        var json = @"{ ""users"": [ 
            { ""firstName"": ""John"", ""lastName"": ""Doe"", ""email"": ""john.doe@example.com"", ""phone"": ""1111111111"" }, 
            { ""firstName"": ""Jane"", ""lastName"": ""Smith"", ""email"": ""john.doe@example.com"", ""phone"": ""2222222222"" } 
        ] }";
        var transferService = CreateServiceWithHttpResponse(json);

        // Act
        var result = await transferService.StartTransferAsync();

        // Assert
        result.SuccessfulRecords.Should().Be(1);
        result.FailedRecords.Should().Be(1);

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
        _errorLogServiceMock.Verify(x => x.AddErrorAsync(It.Is<ErrorLogDto>(e => 
            e.ErrorField == "Email" && e.ErrorMessage == "Duplicate email in transfer package.")), Times.Once);
    }

    [Fact]
    public async Task StartTransferAsync_UserAlreadyExists_ShouldCreateErrorLog()
    {
        // Arrange
        var json = @"{ ""users"": [ { ""firstName"": ""John"", ""lastName"": ""Doe"", ""email"": ""john.doe@example.com"", ""phone"": ""+1-202-555-0101"" } ] }";
        var transferService = CreateServiceWithHttpResponse(json);

        // Bu test için varsayılan davranışı ezerek kullanıcının veritabanında var olduğunu belirtiyoruz
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("john.doe@example.com"))
            .ReturnsAsync(new User
            {
                Id = Guid.NewGuid(),
                Name = "Existing User",
                Email = "john.doe@example.com"
            });

        // Act
        var result = await transferService.StartTransferAsync();

        // Assert
        result.SuccessfulRecords.Should().Be(0);
        result.FailedRecords.Should().Be(1);

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
        _errorLogServiceMock.Verify(x => x.AddErrorAsync(It.Is<ErrorLogDto>(e => 
            e.ErrorField == "Email" && e.ErrorMessage == "User already exists.")), Times.Once);
    }

    [Fact]
    public async Task StartTransferAsync_ApiReturnsError_ShouldThrowException()
    {
        // Arrange
        var transferService = CreateServiceWithHttpResponse(string.Empty, HttpStatusCode.InternalServerError);

        // Act
        var action = async () => await transferService.StartTransferAsync();

        // Assert
        await action.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task StartTransferAsync_UsersAreNull_ShouldThrowException()
    {
        // Arrange
        var transferService = CreateServiceWithHttpResponse("null");

        // Act
        var action = async () => await transferService.StartTransferAsync();

        // Assert
        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Users could not be retrieved from the external API.");
    }

    #region Helper Methods

    private TransferService CreateServiceWithHttpResponse(string jsonContent, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            });

        var httpClient = new HttpClient(handlerMock.Object);

        return new TransferService(
            _userRepositoryMock.Object,
            httpClient,
            _transferLogServiceMock.Object,
            _errorLogServiceMock.Object);
    }

    #endregion
}