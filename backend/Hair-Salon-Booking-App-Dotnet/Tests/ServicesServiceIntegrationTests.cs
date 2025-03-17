using Xunit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ServicesServiceIntegrationTests : IDisposable
{
    private readonly IServicesService _servicesService;
    private readonly IServicesRepository _servicesRepository;
    private readonly AppDbContext _dbContext;

    public ServicesServiceIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _servicesRepository = new ServicesRepository(_dbContext);
        _servicesService = new ServicesService(_servicesRepository);
    }

    [Fact]
    public async Task GetAllServices_ShouldReturnServices_WhenServicesExist()
    {
        Console.WriteLine("Testing if GetAllServices returns existing services from the database.");

        _dbContext.Services.AddRange(new List<Service>
        {
            new Service { Id = 1, Name = "Service 1" },
            new Service { Id = 2, Name = "Service 2" }
        });
        await _dbContext.SaveChangesAsync();

        var result = await _servicesService.GetAllServices();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, s => s.Name == "Service 1");
        Assert.Contains(result, s => s.Name == "Service 2");
    }

    [Fact]
    public async Task GetAllServices_ShouldReturnEmptyList_WhenNoServicesExist()
    {
        Console.WriteLine("Testing if GetAllServices returns an empty list when no services exist.");

        var result = await _servicesService.GetAllServices();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}
