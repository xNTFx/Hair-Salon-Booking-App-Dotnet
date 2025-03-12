using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// get port from configuration or set default to 8080
var port = builder.Configuration.GetValue<int?>("Server:Port") ?? 8080;

// configure services
builder.Services.ConfigureCors();
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureRepositories();
builder.Services.ConfigureJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

// configure middleware
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// run the application on the specified port
app.Run($"http://0.0.0.0:{port}");
