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
                    policy.WithOrigins("http://localhost:5173")
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
