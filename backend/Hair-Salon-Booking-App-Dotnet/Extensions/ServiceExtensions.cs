using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                    policy.WithOrigins("http://localhost:5173")
                          .AllowCredentials()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
        });
    }

    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var logger = loggerFactory.CreateLogger("DatabaseConfiguration");
        
        logger.LogInformation("Using database connection string: {ConnectionString}", connectionString);

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.UseLoggerFactory(loggerFactory);
            options.EnableSensitiveDataLogging();
        });
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

    public static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["AccessTokenSecret"]);
        var logger = loggerFactory.CreateLogger("JwtConfiguration");
        
        logger.LogInformation("Configuring JWT Authentication");
        
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
