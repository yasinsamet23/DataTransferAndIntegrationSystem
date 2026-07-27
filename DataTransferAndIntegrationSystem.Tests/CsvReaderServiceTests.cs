using System.Text;
using DataTransferAndIntegrationSystem.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace DataTransferAndIntegrationSystem.Tests.Services;

public class CsvReaderServiceTests
{
    private readonly CsvReaderService _csvReaderService;

    public CsvReaderServiceTests()
    {
        _csvReaderService = new CsvReaderService();
    }

    #region Helper Methods

    private MemoryStream CreateStream(string content)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(content));
    }

    #endregion

    #region ReadUsersAsync Tests

    [Fact]
    public async Task ReadUsersAsync_ValidCsv_ShouldReturnUsers()
    {
        // Arrange
        var csv = 
@"FirstName,LastName,Email,Phone
John,Doe,john@test.com,5551112233";

        using var stream = CreateStream(csv);

        // Act
        var result = await _csvReaderService.ReadUsersAsync(stream);

        // Assert
        result.Should().HaveCount(1);
        result[0].FirstName.Should().Be("John");
        result[0].LastName.Should().Be("Doe");
        result[0].Email.Should().Be("john@test.com");
        result[0].Phone.Should().Be("5551112233");
    }

    [Fact]
    public async Task ReadUsersAsync_MultipleUsers_ShouldReturnAllUsers()
    {
        // Arrange
        var csv = 
@"FirstName,LastName,Email,Phone
John,Doe,john@test.com,5551112233
Jane,Smith,jane@test.com,5554445566";

        using var stream = CreateStream(csv);

        // Act
        var result = await _csvReaderService.ReadUsersAsync(stream);

        // Assert
        result.Should().HaveCount(2);
        result[1].FirstName.Should().Be("Jane");
        result[1].LastName.Should().Be("Smith");
        result[1].Email.Should().Be("jane@test.com");
        result[1].Phone.Should().Be("5554445566");
    }

    #endregion

    #region ValidateHeader Tests

    [Fact]
    public void ValidateHeader_ValidHeader_ShouldNotThrowException()
    {
        // Arrange
        var csv = 
@"FirstName,LastName,Email,Phone
John,Doe,john@test.com,5551112233";

        using var stream = CreateStream(csv);

        // Act
        var action = () => _csvReaderService.ValidateHeader(stream);

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void ValidateHeader_MissingColumn_ShouldThrowException()
    {
        // Arrange
        var csv = 
@"FirstName,LastName,Email
John,Doe,john@test.com";

        using var stream = CreateStream(csv);

        // Act
        var action = () => _csvReaderService.ValidateHeader(stream);

        // Assert
        action.Should()
            .Throw<Exception>()
            .WithMessage("CSV format is invalid. Expected columns: FirstName, LastName, Email, Phone.");
    }

    [Fact]
    public void ValidateHeader_WrongOrder_ShouldThrowException()
    {
        // Arrange
        var csv = 
@"Email,FirstName,LastName,Phone
john@test.com,John,Doe,5551112233";

        using var stream = CreateStream(csv);

        // Act
        var action = () => _csvReaderService.ValidateHeader(stream);

        // Assert
        action.Should()
            .Throw<Exception>()
            .WithMessage("CSV format is invalid. Expected columns: FirstName, LastName, Email, Phone.");
    }

    [Fact]
    public void ValidateHeader_InvalidHeader_ShouldThrowException()
    {
        // Arrange
        var csv = 
@"Name,Surname,Mail,Telephone
John,Doe,john@test.com,5551112233";

        using var stream = CreateStream(csv);

        // Act
        var action = () => _csvReaderService.ValidateHeader(stream);

        // Assert
        action.Should()
            .Throw<Exception>()
            .WithMessage("CSV format is invalid. Expected columns: FirstName, LastName, Email, Phone.");
    }

    [Fact]
    public void ValidateHeader_EmptyFile_ShouldThrowException()
    {
        // Arrange
        var csv = "";
        using var stream = CreateStream(csv);

        // Act
        var action = () => _csvReaderService.ValidateHeader(stream);

        // Assert
        action.Should().Throw<Exception>();
    }

    #endregion
}