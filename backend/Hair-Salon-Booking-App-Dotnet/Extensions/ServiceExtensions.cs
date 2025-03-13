using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy =>
                {
                    policy.WithOrigins(
                        "https://hairsalonbookingapp-dotnet.pawelsobon.pl",
                        "https://www.hairsalonbookingapp-dotnet.pawelsobon.pl"
                    )
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
        });
    }

    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
    }

    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IEmployeeService, EmployeeService>();

        services.AddScoped<IServicesRepository, ServicesRepository>();
        services.AddScoped<IServicesService, ServicesService>();

        services.AddScoped<IAvailableHoursRepository, AvailableHoursRepository>();
        services.AddScoped<IAvailableHoursService, AvailableHoursService>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<AuthService>();

        services.AddScoped<IReservationsRepository, ReservationsRepository>();
        services.AddScoped<IReservationsService, ReservationsService>();
    }

    public static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["AccessTokenSecret"]);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };
            });

        services.AddSingleton<JwtUtil>();
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

var builder = WebApplication.CreateBuilder(args);

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
