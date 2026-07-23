using DataTransferAndIntegrationSystem.Application.Interfaces;
using Hangfire;

namespace DataTransferAndIntegrationSystem.Infrastructure.Services;

public class TransferSchedulerService : ITransferSchedulerService
{
    public void ScheduleNightlyTransfer()
    {
        var turkeyTimeZone =
    TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");

        RecurringJob.AddOrUpdate<ITransferService>(
    "night-transfer",
    x => x.StartNightlyTransferAsync(),
    Cron.Daily(5, 30),
    new RecurringJobOptions
    {
        TimeZone = turkeyTimeZone
    });
    }
}