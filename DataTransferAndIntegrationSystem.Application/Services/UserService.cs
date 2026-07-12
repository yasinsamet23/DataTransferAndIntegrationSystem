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

        return users.Select(user => new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            CreatedDate = user.CreatedDate
        }).ToList();
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            return null;

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            CreatedDate = user.CreatedDate
        };
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        if (string.IsNullOrWhiteSpace(createUserDto.Name))
        {
            throw new Exception("The Name field is required.");
        }

        if (string.IsNullOrWhiteSpace(createUserDto.Email))
        {
            throw new Exception("Email is required.");
        }

        if (!Regex.IsMatch(
            createUserDto.Email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new Exception("Invalid email format.");
        }

        var existingUser = await _userRepository.GetByEmailAsync(createUserDto.Email);

        if (existingUser != null)
            throw new Exception("This email address is already registered.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = createUserDto.Name,
            Email = createUserDto.Email,
            Phone = createUserDto.Phone,
            CreatedDate = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            CreatedDate = user.CreatedDate
        };
    }

    public async Task UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
    {
        if (string.IsNullOrWhiteSpace(updateUserDto.Name))
        {
            throw new Exception("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(updateUserDto.Email))
        {
            throw new Exception("Email is required.");
        }

        if (!Regex.IsMatch(
                updateUserDto.Email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new Exception("Invalid email format.");
        }
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new Exception("User not found.");
        
        
        var existingUser = await _userRepository.GetByEmailAsync(updateUserDto.Email);

        if (existingUser != null && existingUser.Id != user.Id)
        {
            throw new Exception("A user with this email already exists.");
        }


        user.Name = updateUserDto.Name;
        user.Email = updateUserDto.Email;
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
}