using DataTransferAndIntegrationSystem.Persistence;
using Microsoft.EntityFrameworkCore;
using DataTransferAndIntegrationSystem.Application.Interfaces;
using DataTransferAndIntegrationSystem.Application.Services;
using DataTransferAndIntegrationSystem.Persistence.Repositories;
using DataTransferAndIntegrationSystem.Infrastructure.Services;
using DataTransferAndIntegrationSystem.API.Middleware;
using Hangfire;
using Hangfire.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITransferService, TransferService>();
builder.Services.AddSingleton<IMockarooSettings, MockarooSettings>();
builder.Services.AddScoped<ITransferSchedulerService, TransferSchedulerService>();
builder.Services.AddScoped<ITransferLogRepository, TransferLogRepository>();
builder.Services.AddScoped<ITransferLogService, TransferLogService>();
builder.Services.AddScoped<IErrorLogRepository, ErrorLogRepository>();
builder.Services.AddScoped<IErrorLogService, ErrorLogService>();
builder.Services.AddScoped<ICsvReaderService, CsvReaderService>();

builder.Services.AddHttpClient();

builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(
            builder.Configuration.GetConnectionString("DefaultConnection")));
});

builder.Services.AddHangfireServer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("ReactPolicy");
app.UseHttpsRedirection();
app.UseHangfireDashboard("/hangfire");
using (var scope = app.Services.CreateScope())
{
    var scheduler =
        scope.ServiceProvider.GetRequiredService<ITransferSchedulerService>();

    scheduler.ScheduleNightlyTransfer();
}
app.MapControllers();



app.Run();


