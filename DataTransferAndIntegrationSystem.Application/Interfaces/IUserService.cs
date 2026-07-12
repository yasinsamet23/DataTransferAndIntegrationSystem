using DataTransferAndIntegrationSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataTransferAndIntegrationSystem.Application.Interfaces;
             
public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();

    Task<UserDto?> GetUserByIdAsync(Guid id);

    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);

    Task UpdateUserAsync(Guid id, UpdateUserDto updateUserDto);

    Task DeleteUserAsync(Guid id);
}