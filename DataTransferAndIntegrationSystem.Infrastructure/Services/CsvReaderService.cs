using CsvHelper;
using DataTransferAndIntegrationSystem.Application.DTOs;
using DataTransferAndIntegrationSystem.Application.Interfaces;
using System.Globalization;

namespace DataTransferAndIntegrationSystem.Infrastructure.Services;

public class CsvReaderService : ICsvReaderService
{
    public Task<List<ExternalUserDto>> ReadUsersAsync(Stream stream)
    {
        ValidateHeader(stream);
        
        using var reader = new StreamReader(stream);

        using var csv = new CsvReader(
            reader,
            CultureInfo.InvariantCulture);

        var users = csv
            .GetRecords<ExternalUserDto>()
            .ToList();

        return Task.FromResult(users);
    }

    public void ValidateHeader(Stream stream)
    {
        stream.Position = 0;

        using var reader = new StreamReader(
            stream,
            leaveOpen: true);

        using var csv =
            new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Read();
        csv.ReadHeader();

        var headers = csv.HeaderRecord;

        string[] expected =
        {
        "FirstName",
        "LastName",
        "Email",
        "Phone"
    };

        if (headers == null)
            throw new Exception("CSV header could not be read.");

        if (!headers.SequenceEqual(expected))
            throw new Exception(
                "CSV format is invalid. Expected columns: FirstName, LastName, Email, Phone.");

        stream.Position = 0;
    }


}