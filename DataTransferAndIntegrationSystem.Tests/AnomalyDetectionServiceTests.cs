using FluentAssertions;
using Xunit;
using DataTransferAndIntegrationSystem.Application.DTOs;
using DataTransferAndIntegrationSystem.Infrastructure.Services;

namespace DataTransferAndIntegrationSystem.Tests.Services;

public class AnomalyDetectionServiceTests
{
    private readonly AnomalyDetectionService _service;

    public AnomalyDetectionServiceTests()
    {
        _service = new AnomalyDetectionService();
    }

    #region Helper Methods

    // Varsayılan geçerli bir kullanıcı döner. 
    // Testler sadece değiştirmek istedikleri alanları parametre olarak gönderir.
    private ExternalUserDto CreateTestUser(
        string firstName = "John",
        string lastName = "Doe",
        string email = "john@gmail.com",
        string phone = "5551234567")
    {
        return new ExternalUserDto
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone
        };
    }

    #endregion

    #region General Tests

    [Fact]
    public void ValidateUser_ValidUser_ShouldReturnNoErrors()
    {
        // Arrange
        var user = CreateTestUser(email: "john.doe@gmail.com");

        // Act
        var result = _service.ValidateUser(user);

        // Assert
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateUser_MultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var user = CreateTestUser(
            firstName: "Test123", 
            lastName: "", 
            email: "fake@gmail.com", 
            phone: "1111111111");

        // Act
        var result = _service.ValidateUser(user);

        // Assert
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().Contain(x => x.Field == "Name");
        result.Errors.Should().Contain(x => x.Field == "Email");
        result.Errors.Should().Contain(x => x.Field == "Phone");
    }

    #endregion

    #region Name Validation Tests

    [Fact]
    public void ValidateUser_NameTooShort_ShouldReturnError()
    {
        // Arrange
        var user = CreateTestUser(firstName: "A", lastName: "");

        // Act
        var result = _service.ValidateUser(user);

        // Assert
        result.Errors.Should().ContainSingle();
        result.Errors[0].Field.Should().Be("Name");
        result.Errors[0].Message.Should().Be("Name is too short.");
    }

    [Fact]
    public void ValidateUser_NameContainsNumbers_ShouldReturnError()
    {
        // Arrange
        var user = CreateTestUser(firstName: "John123");

        // Act
        var result = _service.ValidateUser(user);

        // Assert
        result.Errors.Should().ContainSingle();
        result.Errors[0].Field.Should().Be("Name");
        result.Errors[0].Message.Should().Be("Name contains numbers.");
    }

    [Theory]
    [InlineData("test")]
    [InlineData("admin")]
    [InlineData("user")]
    [InlineData("unknown")]
    [InlineData("asdf")]
    [InlineData("qwerty")]
    [InlineData("asdasd")]
    public void ValidateUser_SuspiciousName_ShouldReturnError(string suspiciousName)
    {
        // Arrange
        var user = CreateTestUser(firstName: suspiciousName, lastName: "Smith");

        // Act
        var result = _service.ValidateUser(user);

        // Assert
        result.Errors.Should().ContainSingle();
        result.Errors[0].Field.Should().Be("Name");
        result.Errors[0].Message.Should().Be("Suspicious name detected.");
    }

    [Fact]
    public void ValidateUser_RepeatedPatternName_ShouldReturnError()
    {
        // Arrange
        var user = CreateTestUser(firstName: "abcabc", lastName: "");

        // Act
        var result = _service.ValidateUser(user);

        // Assert
        result.Errors.Should().ContainSingle();
        result.Errors[0].Field.Should().Be("Name");
        result.Errors[0].Message.Should().Be("Name contains repeated pattern.");
    }

    [Fact]
    public void ValidateUser_RepeatedCharacters_ShouldReturnError()
    {
        // Arrange
        var user = CreateTestUser(firstName: "aaaaaa", lastName: "");

        // Act
        var result = _service.ValidateUser(user);

        // Assert
        result.Errors.Should().ContainSingle();
        result.Errors[0].Field.Should().Be("Name");
        result.Errors[0].Message.Should().Be("Name contains repeated characters.");
    }

    [Fact]
    public void ValidateUser_NameWithTurkishCharacters_ShouldReturnNoErrors()
    {
        // Arrange
        var user = CreateTestUser(firstName: "Çağlar", lastName: "Şahin", email: "caglar@gmail.com");

        // Act
        var result = _service.ValidateUser(user);

        // Assert
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateUser_SuspiciousName_IgnoresCase_ShouldReturnError()
    {
        // Arrange
        var user = CreateTestUser(firstName: "TeSt", lastName: "User");

        // Act
        var result = _service.ValidateUser(user);

        // Assert
        result.Errors.Should().ContainSingle();
        result.Errors[0].Field.Should().Be("Name");
        result.Errors[0].Message.Should().Be("Suspicious name detected.");
    }

    #endregion

    #region Email Validation Tests

    [Theory]
    [InlineData("test@gmail.com")]
    [InlineData("admin@yahoo.com")]
    [InlineData("example@hotmail.com")]
    [InlineData("fake@mail.com")]
    public void ValidateUser_SuspiciousEmail_ShouldReturnError(string email)
    {
        // Arrange
        var user = CreateTestUser(email: email);

        // Act
        var result = _service.ValidateUser(user);

        // Assert
        result.Errors.Should().ContainSingle();
        result.Errors[0].Field.Should().Be("Email");
        result.Errors[0].Message.Should().Be("Suspicious email detected.");
    }

    [Fact]
    public void ValidateUser_SuspiciousEmail_IgnoresCase_ShouldReturnError()
    {
        // Arrange
        var user = CreateTestUser(email: "AdMiN@gmail.com");

        // Act
        var result = _service.ValidateUser(user);

        // Assert
        result.Errors.Should().ContainSingle();
        result.Errors[0].Field.Should().Be("Email");
        result.Errors[0].Message.Should().Be("Suspicious email detected.");
    }

    #endregion

    #region Phone Validation Tests

    [Fact]
    public void ValidateUser_EmptyPhone_ShouldReturnError()
    {
        // Arrange
        var user = CreateTestUser(phone: "");

        // Act
        var result = _service.ValidateUser(user);

        // Assert
        result.Errors.Should().ContainSingle();
        result.Errors[0].Field.Should().Be("Phone");
        result.Errors[0].Message.Should().Be("Phone number is empty.");
    }

    [Fact]
    public void ValidateUser_ShortPhone_ShouldReturnError()
    {
        // Arrange
        var user = CreateTestUser(phone: "12345");

        // Act
        var result = _service.ValidateUser(user);

        // Assert
        result.Errors.Should().ContainSingle();
        result.Errors[0].Field.Should().Be("Phone");
        result.Errors[0].Message.Should().Be("Phone number is too short.");
    }

    [Fact]
    public void ValidateUser_RepeatedDigitsPhone_ShouldReturnError()
    {
        // Arrange
        var user = CreateTestUser(phone: "1111111111");

        // Act
        var result = _service.ValidateUser(user);

        // Assert
        result.Errors.Should().ContainSingle();
        result.Errors[0].Field.Should().Be("Phone");
        result.Errors[0].Message.Should().Be("Phone number contains repeated digits.");
    }

    [Fact]
    public void ValidateUser_SequentialPhone_ShouldReturnError()
    {
        // Arrange
        var user = CreateTestUser(phone: "1234567890");

        // Act
        var result = _service.ValidateUser(user);

        // Assert
        result.Errors.Should().ContainSingle();
        result.Errors[0].Field.Should().Be("Phone");
        result.Errors[0].Message.Should().Be("Sequential phone number detected.");
    }

    [Fact]
    public void ValidateUser_PhoneWithFormatting_ShouldReturnNoErrors()
    {
        // Arrange
        var user = CreateTestUser(phone: "+90 (555) 123-45-67");

        // Act
        var result = _service.ValidateUser(user);

        // Assert
        result.Errors.Should().BeEmpty();
    }

    #endregion
}