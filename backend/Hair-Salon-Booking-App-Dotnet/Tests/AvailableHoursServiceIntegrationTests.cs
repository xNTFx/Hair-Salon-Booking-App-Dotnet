using Xunit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AvailableHoursServiceIntegrationTests : IDisposable
{
	private readonly IAvailableHoursService _availableHoursService;
	private readonly IAvailableHoursRepository _availableHoursRepository;
	private readonly AppDbContext _dbContext;

	public AvailableHoursServiceIntegrationTests()
	{
		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;

		_dbContext = new AppDbContext(options);
		_availableHoursRepository = new AvailableHoursRepository(_dbContext);
		_availableHoursService = new AvailableHoursService(_availableHoursRepository);
	}

	[Fact]
	public async Task GetAllEmployees_ShouldReturnAvailableHours_WhenDataExists()
	{
		Console.WriteLine("Testing if GetAllEmployees returns available hours when data exists.");

		_dbContext.AvailableHours.AddRange(new List<AvailableHours>
		{
			new AvailableHours { Id = 1, EmployeeId = 1, StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(12) },
			new AvailableHours { Id = 2, EmployeeId = 2, StartTime = TimeSpan.FromHours(10), EndTime = TimeSpan.FromHours(14) }
		});
		await _dbContext.SaveChangesAsync();

		var result = await _availableHoursService.GetAllEmployees();

		Assert.NotNull(result);
		Assert.Equal(2, result.Count());
	}

	[Fact]
	public async Task GetAllEmployees_ShouldReturnEmptyList_WhenNoDataExists()
	{
		Console.WriteLine("Testing if GetAllEmployees returns an empty list when no available hours exist.");

		var result = await _availableHoursService.GetAllEmployees();

		Assert.NotNull(result);
		Assert.Empty(result);
	}

	[Fact]
	public async Task GetAvailableHoursByEmployeeId_ShouldReturnAvailableHours_WhenEmployeeHasAvailability()
	{
		Console.WriteLine("Testing if GetAvailableHoursByEmployeeId returns available hours for a specific employee.");

		var reservationDate = DateTime.UtcNow.Date;
		_dbContext.AvailableHours.AddRange(new List<AvailableHours>
		{
			new AvailableHours { Id = 1, EmployeeId = 1, StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(12) },
			new AvailableHours { Id = 2, EmployeeId = 1, StartTime = TimeSpan.FromHours(13), EndTime = TimeSpan.FromHours(16) }
		});
		await _dbContext.SaveChangesAsync();

		var result = await _availableHoursService.GetAvailableHoursByEmployeeId(1, reservationDate, "1 hour");

		Assert.NotNull(result);
		Assert.Equal(2, result.Count());
	}

	[Fact]
	public async Task GetAvailableHoursByEmployeeId_ShouldReturnEmptyList_WhenNoAvailability()
	{
		Console.WriteLine("Testing if GetAvailableHoursByEmployeeId returns an empty list when employee has no availability.");

		var reservationDate = DateTime.UtcNow.Date;
		var result = await _availableHoursService.GetAvailableHoursByEmployeeId(1, reservationDate, "1 hour");

		Assert.NotNull(result);
		Assert.Empty(result);
	}

	public void Dispose()
	{
		_dbContext.Database.EnsureDeleted();
		_dbContext.Dispose();
	}
}
