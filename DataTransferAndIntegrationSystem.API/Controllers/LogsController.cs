using DataTransferAndIntegrationSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DataTransferAndIntegrationSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly ITransferLogService _transferLogService;

    public LogsController(ITransferLogService transferLogService)
    {
        _transferLogService = transferLogService;
    }

    // GET: api/logs
    [HttpGet]
    public async Task<IActionResult> GetTransferLogs()
    {
        var logs = await _transferLogService.GetAllTransferLogsAsync();

        return Ok(logs);
    }
}