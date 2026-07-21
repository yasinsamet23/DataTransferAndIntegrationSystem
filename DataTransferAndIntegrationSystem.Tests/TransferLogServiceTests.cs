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
        _transferLogRepositoryMock =
            new Mock<ITransferLogRepository>();

        _transferLogService =
            new TransferLogService(
                _transferLogRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllTransferLogsAsync_ShouldReturnTransferLogs()
    {
        // Arrange

        var transferLogs = new List<TransferLog>
    {
        new TransferLog
        {
            Id = Guid.NewGuid(),
            TransferDate = DateTime.UtcNow,
            TotalRecords = 100,
            SuccessCount = 95,
            Status = "Completed"
        },

        new TransferLog
        {
            Id = Guid.NewGuid(),
            TransferDate = DateTime.UtcNow,
            TotalRecords = 50,
            SuccessCount = 50,
            Status = "Completed"
        }
    };

        _transferLogRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(transferLogs);

        // Act

        var result =
            await _transferLogService.GetAllTransferLogsAsync();

        // Assert

        result.Should().NotBeNull();

        result.Should().HaveCount(2);

        result[0].Status.Should().Be("Completed");

        result[1].TotalRecords.Should().Be(50);

        _transferLogRepositoryMock.Verify(
            x => x.GetAllAsync(),
            Times.Once);
    }

    [Fact]
    public async Task GetAllTransferLogsAsync_ShouldReturnEmptyList()
    {
        // Arrange

        _transferLogRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<TransferLog>());

        // Act

        var result =
            await _transferLogService.GetAllTransferLogsAsync();

        // Assert

        result.Should().NotBeNull();

        result.Should().BeEmpty();

        _transferLogRepositoryMock.Verify(
            x => x.GetAllAsync(),
            Times.Once);
    }

    [Fact]
    public async Task AddTransferLogAsync_ShouldAddTransferLog()
    {
        // Arrange

        var dto = new TransferLogDto
        {
            Id = Guid.NewGuid(),
            TransferDate = DateTime.UtcNow,
            TotalRecords = 100,
            SuccessCount = 90,
            Status = "Completed"
        };

        // Act

        await _transferLogService.AddTransferLogAsync(dto);

        // Assert

        _transferLogRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<TransferLog>()),
            Times.Once);

        _transferLogRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task UpdateTransferLogAsync_ShouldUpdateTransferLog()
    {
        // Arrange

        var id = Guid.NewGuid();

        var transferLog = new TransferLog
        {
            Id = id,
            TransferDate = DateTime.UtcNow,
            TotalRecords = 20,
            SuccessCount = 15,
            Status = "Running"
        };

        var dto = new TransferLogDto
        {
            Id = id,
            TransferDate = DateTime.UtcNow,
            TotalRecords = 100,
            SuccessCount = 95,
            Status = "Completed"
        };

        _transferLogRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(transferLog);

        // Act

        await _transferLogService.UpdateTransferLogAsync(dto);

        // Assert

        transferLog.TotalRecords.Should().Be(100);

        transferLog.SuccessCount.Should().Be(95);

        transferLog.Status.Should().Be("Completed");

        _transferLogRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task UpdateTransferLogAsync_TransferLogNotFound_ShouldThrowException()
    {
        // Arrange

        var dto = new TransferLogDto
        {
            Id = Guid.NewGuid(),
            TransferDate = DateTime.UtcNow,
            TotalRecords = 100,
            SuccessCount = 100,
            Status = "Completed"
        };

        _transferLogRepositoryMock
            .Setup(x => x.GetByIdAsync(dto.Id))
            .ReturnsAsync((TransferLog?)null);

        // Act

        var action = async () =>
            await _transferLogService.UpdateTransferLogAsync(dto);

        // Assert

        await action.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Transfer log not found.");
    }






}