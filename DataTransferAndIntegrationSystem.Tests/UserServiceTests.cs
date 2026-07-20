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

        _userService = new UserService(
            _userRepositoryMock.Object);
    }
    // CreateUserAsync
    [Fact]
    public async Task CreateUserAsync_NameIsEmpty_ShouldThrowException()
    {
        var dto = new CreateUserDto
        {
            Name = "",
            Email = "test@test.com",
            Phone = "123456"
        };

        var action = async () =>
            await _userService.CreateUserAsync(dto);

        await action.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Name is required.");
    }

    [Fact]
    public async Task CreateUserAsync_EmailIsEmpty_ShouldThrowException()
    {
        var dto = new CreateUserDto
        {
            Name = "Yasin",
            Email = "",
            Phone = "123"
        };

        var action = async () =>
            await _userService.CreateUserAsync(dto);

        await action.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Email is required.");
    }

    [Fact]
    public async Task CreateUserAsync_InvalidEmail_ShouldThrowException()
    {
        var dto = new CreateUserDto
        {
            Name = "Yasin",
            Email = "invalidmail",
            Phone = "123"
        };

        var action = async () =>
            await _userService.CreateUserAsync(dto);

        await action.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Invalid email format.");
    }


    [Fact]
    public async Task CreateUserAsync_EmailAlreadyExists_ShouldThrowException()
    {
        var dto = new CreateUserDto
        {
            Name = "Yasin",
            Email = "test@test.com",
            Phone = "123"
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(dto.Email))
            .ReturnsAsync(new User());

        var action = async () =>
            await _userService.CreateUserAsync(dto);

        await action.Should()
            .ThrowAsync<Exception>()
            .WithMessage("This email address is already registered.");
    }

    [Fact]
    public async Task CreateUserAsync_ValidUser_ShouldCreateUser()
    {
        var dto = new CreateUserDto
        {
            Name = "Yasin",
            Email = "test@test.com",
            Phone = "555"
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);

        var result =
            await _userService.CreateUserAsync(dto);

        result.Should().NotBeNull();

        result.Name.Should().Be(dto.Name);

        result.Email.Should().Be(dto.Email);

        _userRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<User>()),
            Times.Once);

        _userRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }
    // GetAllUsersAsync
    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnUsers()
    {
        // Arrange

        var users = new List<User>
    {
        new User
        {
            Id = Guid.NewGuid(),
            Name = "Yasin",
            Email = "yasin@test.com",
            Phone = "111",
            CreatedDate = DateTime.UtcNow
        },

        new User
        {
            Id = Guid.NewGuid(),
            Name = "Ahmet",
            Email = "ahmet@test.com",
            Phone = "222",
            CreatedDate = DateTime.UtcNow
        }
    };

        _userRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(users);

        // Act

        var result = await _userService.GetAllUsersAsync();

        // Assert

        result.Should().NotBeNull();

        result.Should().HaveCount(2);

        result[0].Name.Should().Be("Yasin");

        result[1].Name.Should().Be("Ahmet");

        _userRepositoryMock.Verify(
            x => x.GetAllAsync(),
            Times.Once);
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange

        _userRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<User>());

        // Act

        var result = await _userService.GetAllUsersAsync();

        // Assert

        result.Should().NotBeNull();

        result.Should().BeEmpty();

        _userRepositoryMock.Verify(
            x => x.GetAllAsync(),
            Times.Once);
    }
    // GetUserByIdAsync
    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser()
    {
        // Arrange

        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Name = "Yasin",
            Email = "yasin@test.com",
            Phone = "555",
            CreatedDate = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act

        var result = await _userService.GetUserByIdAsync(userId);

        // Assert

        result.Should().NotBeNull();

        result!.Id.Should().Be(user.Id);

        result.Name.Should().Be(user.Name);

        result.Email.Should().Be(user.Email);

        result.Phone.Should().Be(user.Phone);

        result.CreatedDate.Should().Be(user.CreatedDate);

        _userRepositoryMock.Verify(
            x => x.GetByIdAsync(userId),
            Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_UserNotFound_ShouldReturnNull()
    {
        // Arrange

        var userId = Guid.NewGuid();

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act

        var result = await _userService.GetUserByIdAsync(userId);

        // Assert

        result.Should().BeNull();

        _userRepositoryMock.Verify(
            x => x.GetByIdAsync(userId),
            Times.Once);
    }

    //UpdateUserAsync
    [Fact]
    public async Task UpdateUserAsync_UserNotFound_ShouldThrowException()
    {
        // Arrange

        var userId = Guid.NewGuid();

        var dto = new UpdateUserDto
        {
            Name = "Yasin",
            Email = "yasin@test.com",
            Phone = "555"
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act

        var action = async () =>
            await _userService.UpdateUserAsync(userId, dto);

        // Assert

        await action.Should()
            .ThrowAsync<Exception>()
            .WithMessage("User not found.");
    }

    [Fact]
    public async Task UpdateUserAsync_EmailAlreadyExists_ShouldThrowException()
    {
        // Arrange

        var userId = Guid.NewGuid();

        var dto = new UpdateUserDto
        {
            Name = "Yasin",
            Email = "test@test.com",
            Phone = "555"
        };

        var currentUser = new User
        {
            Id = userId,
            Name = "Old Name",
            Email = "old@test.com",
            Phone = "111"
        };

        var anotherUser = new User
        {
            Id = Guid.NewGuid(),
            Name = "Ahmet",
            Email = "test@test.com",
            Phone = "999"
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(currentUser);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(dto.Email))
            .ReturnsAsync(anotherUser);

        // Act

        var action = async () =>
            await _userService.UpdateUserAsync(userId, dto);

        // Assert

        await action.Should()
            .ThrowAsync<Exception>()
            .WithMessage("A user with this email already exists.");
    }

    [Fact]
    public async Task UpdateUserAsync_SameEmail_ShouldUpdateUser()
    {
        // Arrange

        var userId = Guid.NewGuid();

        var dto = new UpdateUserDto
        {
            Name = "New Name",
            Email = "same@test.com",
            Phone = "999"
        };

        var user = new User
        {
            Id = userId,
            Name = "Old Name",
            Email = "same@test.com",
            Phone = "111"
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(dto.Email))
            .ReturnsAsync(user);

        // Act

        await _userService.UpdateUserAsync(userId, dto);

        // Assert

        user.Name.Should().Be(dto.Name);

        user.Email.Should().Be(dto.Email);

        user.Phone.Should().Be(dto.Phone);

        _userRepositoryMock.Verify(
            x => x.Update(user),
            Times.Once);

        _userRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_ValidData_ShouldUpdateUser()
    {
        // Arrange

        var userId = Guid.NewGuid();

        var dto = new UpdateUserDto
        {
            Name = "New Name",
            Email = "new@test.com",
            Phone = "999"
        };

        var user = new User
        {
            Id = userId,
            Name = "Old Name",
            Email = "old@test.com",
            Phone = "111"
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);

        // Act

        await _userService.UpdateUserAsync(userId, dto);

        // Assert

        user.Name.Should().Be(dto.Name);

        user.Email.Should().Be(dto.Email);

        user.Phone.Should().Be(dto.Phone);

        _userRepositoryMock.Verify(
            x => x.Update(user),
            Times.Once);

        _userRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    // DeleteUserAsync
    [Fact]
    public async Task DeleteUserAsync_UserNotFound_ShouldThrowException()
    {
        // Arrange

        var userId = Guid.NewGuid();

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act

        var action = async () =>
            await _userService.DeleteUserAsync(userId);

        // Assert

        await action.Should()
            .ThrowAsync<Exception>()
            .WithMessage("User not found.");
    }

    [Fact]
    public async Task DeleteUserAsync_ExistingUser_ShouldDeleteUser()
    {
        // Arrange

        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Name = "Yasin",
            Email = "yasin@test.com",
            Phone = "555",
            CreatedDate = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act

        await _userService.DeleteUserAsync(userId);

        // Assert

        _userRepositoryMock.Verify(
            x => x.Delete(user),
            Times.Once);

        _userRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

}