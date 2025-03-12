// Program.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

try
{
    // Get port from configuration
    var port = builder.Configuration.GetValue<int?>("Server:Port") ?? 8080;
    Console.WriteLine($"Starting application on port: {port}");

    // Configure services with validation
    builder.Services.ConfigureCors();
    builder.Services.ConfigureDatabase(builder.Configuration);
    builder.Services.ConfigureRepositories();
    builder.Services.ConfigureJwtAuthentication(builder.Configuration);
    
    builder.Services.AddAuthorization();
    builder.Services.AddControllers();
    builder.Services.AddHostedService<ReservationsScheduler>();

    var app = builder.Build();

    // Middleware pipeline
    app.UseRouting();
    app.UseCors("AllowFrontend");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // Global error handling
    app.UseExceptionHandler("/error");
    app.Map("/error", HandleGlobalError);

    Console.WriteLine("Application configured successfully");
    app.Run($"http://0.0.0.0:{port}");
}
catch (Exception ex)
{
    Console.WriteLine($"Fatal startup error: {ex}");
    throw;
}

static IResult HandleGlobalError(HttpContext context)
{
    var exceptionHandler = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogError(exceptionHandler?.Error, "Global error handler caught exception");
    return Results.Problem("An unexpected error occurred. Please try again later.");
}
