using FluentAssertions;
using Moq;
using Xunit;
using DataTransferAndIntegrationSystem.Application.DTOs;
using DataTransferAndIntegrationSystem.Application.Interfaces;
using DataTransferAndIntegrationSystem.Application.Services;
using DataTransferAndIntegrationSystem.Domain.Entities;

namespace DataTransferAndIntegrationSystem.Tests.Services;

public class TransferLogServiceTests
{
    private readonly Mock<ITransferLogRepository> _transferLogRepositoryMock;
    private readonly TransferLogService _transferLogService;

    public TransferLogServiceTests()
    {
        _transferLogRepositoryMock = new Mock<ITransferLogRepository>();
        _transferLogService = new TransferLogService(_transferLogRepositoryMock.Object);
    }

    #region Helper Methods

    private TransferLog CreateTransferLog(Guid id, int totalRecords, int successCount, string status)
    {
        return new TransferLog
        {
            Id = id,
            TransferDate = DateTime.UtcNow,
            TotalRecords = totalRecords,
            SuccessCount = successCount,
            Status = status
        };
    }

    private TransferLogDto CreateTransferLogDto(Guid id, int totalRecords, int successCount, string status)
    {
        return new TransferLogDto
        {
            Id = id,
            TransferDate = DateTime.UtcNow,
            TotalRecords = totalRecords,
            SuccessCount = successCount,
            Status = status
        };
    }

    #endregion

    #region Read (Get) Operations Tests

    [Fact]
    public async Task GetAllTransferLogsAsync_ShouldReturnTransferLogs()
    {
        // Arrange
        var transferLogs = new List<TransferLog>
        {
            CreateTransferLog(Guid.NewGuid(), 100, 95, "Completed"),
            CreateTransferLog(Guid.NewGuid(), 50, 50, "Completed")
        };

        _transferLogRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(transferLogs);

        // Act
        var result = await _transferLogService.GetAllTransferLogsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Status.Should().Be("Completed");
        result[1].TotalRecords.Should().Be(50);

        _transferLogRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllTransferLogsAsync_ShouldReturnEmptyList()
    {
        // Arrange
        _transferLogRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<TransferLog>());

        // Act
        var result = await _transferLogService.GetAllTransferLogsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _transferLogRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    #endregion

    #region Write (Insert) Operations Tests

    [Fact]
    public async Task AddTransferLogAsync_ShouldAddTransferLog()
    {
        // Arrange
        var dto = CreateTransferLogDto(Guid.NewGuid(), 100, 90, "Completed");

        // Act
        await _transferLogService.AddTransferLogAsync(dto);

        // Assert
        _transferLogRepositoryMock.Verify(x => x.AddAsync(It.IsAny<TransferLog>()), Times.Once);
        _transferLogRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task BulkInsertTransferLogsAsync_ShouldBulkInsertTransferLogs()
    {
        // Arrange
        var dtos = new List<TransferLogDto>
        {
            CreateTransferLogDto(Guid.NewGuid(), 10, 8, "Completed With Errors")
        };

        // Act
        await _transferLogService.BulkInsertTransferLogsAsync(dtos);

        // Assert
        _transferLogRepositoryMock.Verify(x => x.BulkInsertAsync(
            It.Is<List<TransferLog>>(logs => 
                logs.Count == 1 &&
                logs[0].TotalRecords == 10 &&
                logs[0].Status == "Completed With Errors")), 
            Times.Once);
    }

    #endregion

    #region Update Operations Tests

    [Fact]
    public async Task UpdateTransferLogAsync_ShouldUpdateTransferLog()
    {
        // Arrange
        var id = Guid.NewGuid();
        var transferLog = CreateTransferLog(id, 20, 15, "Running");
        var dto = CreateTransferLogDto(id, 100, 95, "Completed");

        _transferLogRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(transferLog);

        // Act
        await _transferLogService.UpdateTransferLogAsync(dto);

        // Assert
        transferLog.TotalRecords.Should().Be(100);
        transferLog.SuccessCount.Should().Be(95);
        transferLog.Status.Should().Be("Completed");

        _transferLogRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateTransferLogAsync_TransferLogNotFound_ShouldThrowException()
    {
        // Arrange
        var dto = CreateTransferLogDto(Guid.NewGuid(), 100, 100, "Completed");

        _transferLogRepositoryMock
            .Setup(x => x.GetByIdAsync(dto.Id))
            .ReturnsAsync((TransferLog?)null);

        // Act
        var action = async () => await _transferLogService.UpdateTransferLogAsync(dto);

        // Assert
        await action.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Transfer log not found.");
    }

    #endregion
}
