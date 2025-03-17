using Xunit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class EmployeeServiceIntegrationTests : IDisposable
{
    private readonly IEmployeeService _employeeService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly AppDbContext _dbContext;

    public EmployeeServiceIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _employeeRepository = new EmployeeRepository(_dbContext);
        _employeeService = new EmployeeService(_employeeRepository);
    }

    [Fact]
    public async Task GetAllEmployees_ShouldReturnEmployees_WhenEmployeesExist()
    {
        Console.WriteLine("Testing if GetAllEmployees returns employees from the database when they exist.");

        _dbContext.Employees.AddRange(new List<Employee>
        {
            new Employee { EmployeeId = 1, FirstName = "John", LastName = "Doe" },
            new Employee { EmployeeId = 2, FirstName = "Jane", LastName = "Smith" }
        });
        await _dbContext.SaveChangesAsync();

        var result = await _employeeService.GetAllEmployees();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, e => e.FirstName == "John" && e.LastName == "Doe");
        Assert.Contains(result, e => e.FirstName == "Jane" && e.LastName == "Smith");
    }

    [Fact]
    public async Task GetAllEmployees_ShouldReturnEmptyList_WhenNoEmployeesExist()
    {
        Console.WriteLine("Testing if GetAllEmployees returns an empty list when no employees exist in the database.");

        var result = await _employeeService.GetAllEmployees();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}
