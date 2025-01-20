
using ClinicalTrials.API.Common;
using ClinicalTrials.Application.ClinicalTrial.Commands.UploadFile;
using ClinicalTrials.Application.Common.Behaviors;
using ClinicalTrials.Application.Common.Interfaces.Persistence;
using ClinicalTrials.Infrastructure.Persistence;
using ClinicalTrials.Infrastructure.Persistence.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Serilog.Events;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    Assembly.GetExecutingAssembly(),
    typeof(UploadFileCommandHandler).Assembly
));

builder.Services.AddValidatorsFromAssemblyContaining<UploadFileValidator>();

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddScoped<IClinicalTrialRepository, ClinicalTrialsRepository>();

builder.Services.AddDbContext<ClientTrialsDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Host.UseSerilog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Save the Swagger JSON document to a file
    var swaggerProvider = app.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>();
    var swaggerGenerator = app.Services.GetRequiredService<ISwaggerProvider>();
    var swagger = swaggerGenerator.GetSwagger("v1");
    var json = JsonSerializer.Serialize(swagger, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText("swagger.json", json);
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ClientTrialsDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File(
        path: "Common/Logs/logs.txt",
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: LogEventLevel.Warning,
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
        retainedFileCountLimit: 7 
    )
    .CreateLogger();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

Log.CloseAndFlush();
