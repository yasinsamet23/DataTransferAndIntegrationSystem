using FluentAssertions;
using Moq;
using Xunit;
using DataTransferAndIntegrationSystem.Application.DTOs;
using DataTransferAndIntegrationSystem.Application.Interfaces;
using DataTransferAndIntegrationSystem.Application.Services;
using DataTransferAndIntegrationSystem.Domain.Entities;

namespace DataTransferAndIntegrationSystem.Tests.Services;

public class ErrorLogServiceTests
{
    private readonly Mock<IErrorLogRepository> _errorLogRepositoryMock;

    private readonly ErrorLogService _errorLogService;

    public ErrorLogServiceTests()
    {
        _errorLogRepositoryMock =
            new Mock<IErrorLogRepository>();

        _errorLogService =
            new ErrorLogService(
                _errorLogRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllErrorsAsync_ShouldReturnErrorLogs()
    {
        // Arrange

        var errorLogs = new List<ErrorLog>
    {
        new ErrorLog
        {
            Id = Guid.NewGuid(),
            TransferLogId = Guid.NewGuid(),
            RecordId = Guid.NewGuid(),
            ErrorField = "Email",
            ErrorMessage = "Invalid email.",
            CreatedDate = DateTime.UtcNow
        },

        new ErrorLog
        {
            Id = Guid.NewGuid(),
            TransferLogId = Guid.NewGuid(),
            RecordId = Guid.NewGuid(),
            ErrorField = "FirstName",
            ErrorMessage = "First name is required.",
            CreatedDate = DateTime.UtcNow
        }
    };

        _errorLogRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(errorLogs);

        // Act

        var result =
            await _errorLogService.GetAllErrorsAsync();

        // Assert

        result.Should().NotBeNull();

        result.Should().HaveCount(2);

        result[0].ErrorField.Should().Be("Email");

        result[1].ErrorField.Should().Be("FirstName");

        _errorLogRepositoryMock.Verify(
            x => x.GetAllAsync(),
            Times.Once);
    }

    [Fact]
    public async Task GetAllErrorsAsync_ShouldReturnEmptyList()
    {
        // Arrange

        _errorLogRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<ErrorLog>());

        // Act

        var result =
            await _errorLogService.GetAllErrorsAsync();

        // Assert

        result.Should().NotBeNull();

        result.Should().BeEmpty();

        _errorLogRepositoryMock.Verify(
            x => x.GetAllAsync(),
            Times.Once);
    }

    [Fact]
    public async Task AddErrorAsync_ShouldAddErrorLog()
    {
        // Arrange

        var dto = new ErrorLogDto
        {
            TransferLogId = Guid.NewGuid(),
            RecordId = Guid.NewGuid(),
            ErrorField = "Email",
            ErrorMessage = "Invalid email."
        };

        // Act

        await _errorLogService.AddErrorAsync(dto);

        // Assert

        _errorLogRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<ErrorLog>()),
            Times.Once);

        _errorLogRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

}