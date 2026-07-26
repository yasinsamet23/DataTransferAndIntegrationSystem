using DataTransferAndIntegrationSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CsvHelper;
using System.Globalization;
using DataTransferAndIntegrationSystem.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace DataTransferAndIntegrationSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransferController : ControllerBase
{
    private readonly ITransferService _transferService;
    private readonly ICsvReaderService _csvReaderService;

    public TransferController(
        ITransferService transferService,
        ICsvReaderService csvReaderService)
    {
        _transferService = transferService;
        _csvReaderService = csvReaderService;
    }
    [HttpPost("start")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> StartTransfer()
    {
        var result = await _transferService.StartTransferAsync();

        return Ok(result);
    }

    [HttpPost("upload")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Please select a CSV file.");

        if (Path.GetExtension(file.FileName).ToLower() != ".csv")
            return BadRequest("Only CSV files are allowed.");

        try
        {
            using var stream = file.OpenReadStream();

            var users =
                await _csvReaderService.ReadUsersAsync(stream);

            var result =
                await _transferService.StartCsvTransferAsync(users);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}