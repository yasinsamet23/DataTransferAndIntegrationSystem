using DataTransferAndIntegrationSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DataTransferAndIntegrationSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class ErrorsController : ControllerBase
{
    private readonly IErrorLogService _errorLogService;

    public ErrorsController(IErrorLogService errorLogService)
    {
        _errorLogService = errorLogService;
    }

    // GET: api/errors
    [HttpGet]
    public async Task<IActionResult> GetAllErrors()
    {
        var errors = await _errorLogService.GetAllErrorsAsync();

        return Ok(errors);
    }
}