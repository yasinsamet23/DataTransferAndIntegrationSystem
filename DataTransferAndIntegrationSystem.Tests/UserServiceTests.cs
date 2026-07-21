using Moq;
using Xunit;
using FluentAssertions;
using DataTransferAndIntegrationSystem.Application.DTOs;
using DataTransferAndIntegrationSystem.Application.Interfaces;
using DataTransferAndIntegrationSystem.Application.Services;
using DataTransferAndIntegrationSystem.Domain.Entities;

namespace DataTransferAndIntegrationSystem.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    }

    #region CreateUserAsync Tests

    [Fact]
    public async Task CreateUserAsync_NameIsEmpty_ShouldThrowException()
    {
        // Arrange
        var dto = CreateTestCreateUserDto(name: "");

        // Act
        var action = async () => await _userService.CreateUserAsync(dto);

        // Assert
        await action.Should().ThrowAsync<Exception>().WithMessage("Name is required.");
    }

    [Fact]
    public async Task CreateUserAsync_EmailIsEmpty_ShouldThrowException()
    {
        // Arrange
        var dto = CreateTestCreateUserDto(email: "");

        // Act
        var action = async () => await _userService.CreateUserAsync(dto);

        // Assert
        await action.Should().ThrowAsync<Exception>().WithMessage("Email is required.");
    }

    [Fact]
    public async Task CreateUserAsync_InvalidEmail_ShouldThrowException()
    {
        // Arrange
        var dto = CreateTestCreateUserDto(email: "invalidmail");

        // Act
        var action = async () => await _userService.CreateUserAsync(dto);

        // Assert
        await action.Should().ThrowAsync<Exception>().WithMessage("Invalid email format.");
    }

    [Fact]
    public async Task CreateUserAsync_EmailAlreadyExists_ShouldThrowException()
    {
        // Arrange
        var dto = CreateTestCreateUserDto();
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(dto.Email))
            .ReturnsAsync(new User());

        // Act
        var action = async () => await _userService.CreateUserAsync(dto);

        // Assert
        await action.Should().ThrowAsync<Exception>().WithMessage("This email address is already registered.");
    }

    [Fact]
    public async Task CreateUserAsync_ValidUser_ShouldCreateUser()
    {
        // Arrange
        var dto = CreateTestCreateUserDto();
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.CreateUserAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.Email.Should().Be(dto.Email);

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region GetAllUsersAsync Tests

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnUsers()
    {
        // Arrange
        var users = new List<User>
        {
            CreateTestUser(name: "John", email: "john@test.com", phone: "111"),
            CreateTestUser(name: "Jane", email: "jane@test.com", phone: "222")
        };

        _userRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("John");
        result[1].Name.Should().Be("Jane");

        _userRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<User>());

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _userRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    #endregion

    #region GetUserByIdAsync Tests

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateTestUser(id: userId);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Name.Should().Be(user.Name);
        result.Email.Should().Be(user.Email);
        result.Phone.Should().Be(user.Phone);
        result.CreatedDate.Should().Be(user.CreatedDate);

        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_UserNotFound_ShouldReturnNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        result.Should().BeNull();
        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
    }

    #endregion

    #region UpdateUserAsync Tests

    [Fact]
    public async Task UpdateUserAsync_UserNotFound_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = CreateTestUpdateUserDto();
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act
        var action = async () => await _userService.UpdateUserAsync(userId, dto);

        // Assert
        await action.Should().ThrowAsync<Exception>().WithMessage("User not found.");
    }

    [Fact]
    public async Task UpdateUserAsync_EmailAlreadyExists_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = CreateTestUpdateUserDto(email: "test@test.com");
        var currentUser = CreateTestUser(id: userId, name: "Old Name", email: "old@test.com", phone: "111");
        var anotherUser = CreateTestUser(name: "Jane", email: "test@test.com", phone: "999");

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(currentUser);
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(dto.Email)).ReturnsAsync(anotherUser);

        // Act
        var action = async () => await _userService.UpdateUserAsync(userId, dto);

        // Assert
        await action.Should().ThrowAsync<Exception>().WithMessage("A user with this email already exists.");
    }

    [Fact]
    public async Task UpdateUserAsync_SameEmail_ShouldUpdateUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = CreateTestUpdateUserDto(name: "New Name", email: "same@test.com", phone: "999");
        var user = CreateTestUser(id: userId, name: "Old Name", email: "same@test.com", phone: "111");

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

        // Act
        await _userService.UpdateUserAsync(userId, dto);

        // Assert
        user.Name.Should().Be(dto.Name);
        user.Email.Should().Be(dto.Email);
        user.Phone.Should().Be(dto.Phone);

        _userRepositoryMock.Verify(x => x.Update(user), Times.Once);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_ValidData_ShouldUpdateUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = CreateTestUpdateUserDto(name: "New Name", email: "new@test.com", phone: "999");
        var user = CreateTestUser(id: userId, name: "Old Name", email: "old@test.com", phone: "111");

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);

        // Act
        await _userService.UpdateUserAsync(userId, dto);

        // Assert
        user.Name.Should().Be(dto.Name);
        user.Email.Should().Be(dto.Email);
        user.Phone.Should().Be(dto.Phone);

        _userRepositoryMock.Verify(x => x.Update(user), Times.Once);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region DeleteUserAsync Tests

    [Fact]
    public async Task DeleteUserAsync_UserNotFound_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act
        var action = async () => await _userService.DeleteUserAsync(userId);

        // Assert
        await action.Should().ThrowAsync<Exception>().WithMessage("User not found.");
    }

    [Fact]
    public async Task DeleteUserAsync_ExistingUser_ShouldDeleteUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateTestUser(id: userId);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);

        // Act
        await _userService.DeleteUserAsync(userId);

        // Assert
        _userRepositoryMock.Verify(x => x.Delete(user), Times.Once);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region Helper Methods

    private static CreateUserDto CreateTestCreateUserDto(
        string name = "John", 
        string email = "test@test.com", 
        string phone = "123456")
    {
        return new CreateUserDto { Name = name, Email = email, Phone = phone };
    }

    private static UpdateUserDto CreateTestUpdateUserDto(
        string name = "John", 
        string email = "john@test.com", 
        string phone = "555")
    {
        return new UpdateUserDto { Name = name, Email = email, Phone = phone };
    }

    private static User CreateTestUser(
        Guid? id = null, 
        string name = "John", 
        string email = "john@test.com", 
        string phone = "555")
    {
        return new User
        {
            Id = id ?? Guid.NewGuid(),
            Name = name,
            Email = email,
            Phone = phone,
            CreatedDate = DateTime.UtcNow
        };
    }

    #endregion
}