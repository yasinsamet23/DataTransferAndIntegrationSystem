using DataTransferAndIntegrationSystem.Application.DTOs;
using DataTransferAndIntegrationSystem.Application.Interfaces;
using DataTransferAndIntegrationSystem.Domain.Entities;
using System.Text.RegularExpressions;

namespace DataTransferAndIntegrationSystem.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users
    .Select(MapToUserDto)
    .ToList();
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            return null;

        return MapToUserDto(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        ValidateName(createUserDto.Name);

        ValidateEmail(createUserDto.Email);
        var email = NormalizeEmail(createUserDto.Email);

        var existingUser = await _userRepository.GetByEmailAsync(email);

        if (existingUser != null)
            throw new Exception("This email address is already registered.");

        var user = CreateUser(createUserDto, email);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return MapToUserDto(user);
    }

    public async Task UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
    {
        ValidateName(updateUserDto.Name);

        ValidateEmail(updateUserDto.Email);
        var email = NormalizeEmail(updateUserDto.Email);

        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new Exception("User not found.");


        var existingUser = await _userRepository.GetByEmailAsync(email);

        if (existingUser != null && existingUser.Id != user.Id)
        {
            throw new Exception("A user with this email already exists.");
        }


        user.Name = updateUserDto.Name;
        user.Email = email;
        user.Phone = updateUserDto.Phone;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new Exception("User not found.");

        _userRepository.Delete(user);
        await _userRepository.SaveChangesAsync();
    }

    private UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            CreatedDate = user.CreatedDate
        };
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Name is required.");
        }

    }

    private void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new Exception("Email is required.");
        }

        if (!Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new Exception("Invalid email format.");
        }
    }

    private User CreateUser(
    CreateUserDto dto,
    string email)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = email,
            Phone = dto.Phone,
            CreatedDate = DateTime.UtcNow
        };
    }

    private string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}