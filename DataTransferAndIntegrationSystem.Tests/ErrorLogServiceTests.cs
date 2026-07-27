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
        _errorLogRepositoryMock = new Mock<IErrorLogRepository>();
        _errorLogService = new ErrorLogService(_errorLogRepositoryMock.Object);
    }

    #region Helper Methods

    private ErrorLog CreateErrorLog(string errorField, string errorMessage)
    {
        return new ErrorLog
        {
            Id = Guid.NewGuid(),
            TransferLogId = Guid.NewGuid(),
            RecordId = Guid.NewGuid(),
            ErrorField = errorField,
            ErrorMessage = errorMessage,
            CreatedDate = DateTime.UtcNow
        };
    }

    private ErrorLogDto CreateErrorLogDto(Guid transferLogId, string errorField, string errorMessage)
    {
        return new ErrorLogDto
        {
            Id = Guid.NewGuid(),
            TransferLogId = transferLogId,
            RecordId = Guid.NewGuid(),
            ErrorField = errorField,
            ErrorMessage = errorMessage,
            CreatedDate = DateTime.UtcNow
        };
    }

    #endregion

    #region Read (Get) Operations Tests

    [Fact]
    public async Task GetAllErrorsAsync_ShouldReturnErrorLogs()
    {
        // Arrange
        var errorLogs = new List<ErrorLog>
        {
            CreateErrorLog("Email", "Invalid email."),
            CreateErrorLog("FirstName", "First name is required.")
        };

        _errorLogRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(errorLogs);

        // Act
        var result = await _errorLogService.GetAllErrorsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].ErrorField.Should().Be("Email");
        result[1].ErrorField.Should().Be("FirstName");

        _errorLogRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllErrorsAsync_ShouldReturnEmptyList()
    {
        // Arrange
        _errorLogRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<ErrorLog>());

        // Act
        var result = await _errorLogService.GetAllErrorsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _errorLogRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    #endregion

    #region Write (Insert) Operations Tests

    [Fact]
    public async Task AddErrorAsync_ShouldAddErrorLog()
    {
        // Arrange
        var dto = CreateErrorLogDto(Guid.NewGuid(), "Email", "Invalid email.");

        // Act
        await _errorLogService.AddErrorAsync(dto);

        // Assert
        _errorLogRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ErrorLog>()), Times.Once);
        _errorLogRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task BulkInsertErrorsAsync_ShouldBulkInsertAllErrorLogs()
    {
        // Arrange
        var transferLogId = Guid.NewGuid();
        var dtos = new List<ErrorLogDto>
        {
            CreateErrorLogDto(transferLogId, "Email", "Invalid email."),
            CreateErrorLogDto(transferLogId, "FirstName", "First name is required.")
        };

        // Act
        await _errorLogService.BulkInsertErrorsAsync(dtos);

        // Assert
        _errorLogRepositoryMock.Verify(x => x.BulkInsertAsync(
            It.Is<List<ErrorLog>>(errors => 
                errors.Count == 2 && 
                errors.All(error => error.TransferLogId == transferLogId))), 
            Times.Once);
    }

    #endregion
}
