using DataTransferAndIntegrationSystem.Application.DTOs;

namespace DataTransferAndIntegrationSystem.Application.Interfaces;

public interface IAnomalyDetectionService
{
    AnomalyResultDto ValidateUser(ExternalUserDto user);
}