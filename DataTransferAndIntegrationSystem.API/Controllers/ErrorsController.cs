using DataTransferAndIntegrationSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DataTransferAndIntegrationSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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