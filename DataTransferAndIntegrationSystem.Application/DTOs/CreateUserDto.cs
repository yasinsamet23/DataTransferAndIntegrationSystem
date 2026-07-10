namespace DataTransferAndIntegrationSystem.Application.DTOs;

public class CreateUserDto
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;
}