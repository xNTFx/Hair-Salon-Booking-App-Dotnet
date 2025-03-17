using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

public class ReservationsServiceIntegrationTests : IDisposable
{
    private readonly IReservationsService _reservationsService;
    private readonly IReservationsRepository _reservationsRepository;
    private readonly AppDbContext _dbContext;
    private readonly TransactionScope _transactionScope;

    public ReservationsServiceIntegrationTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        _dbContext = new AppDbContext(options);
        _reservationsRepository = new ReservationsRepository(_dbContext);
        _reservationsService = new ReservationsService(_reservationsRepository);
        _transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    }

    [Fact]
    public async Task SaveReservationAsync_ShouldSaveReservation_WhenValidDataProvided()
    {
        Console.WriteLine("Testing if SaveReservationAsync correctly saves a valid reservation.");

        var service = new Service
        {
            Name = "Haircut",
            Duration = TimeSpan.FromMinutes(60),
            Price = 30.00m
        };

        var employee = new Employee
        {
            FirstName = "John",
            LastName = "Doe"
        };

        _dbContext.Services.Add(service);
        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync();

        var reservation = new Reservation
        {
            UserId = "user123",
            EmployeeId = employee.EmployeeId,
            ServiceId = service.Id,
            ReservationDate = DateTime.UtcNow.Date,
            StartTime = TimeSpan.FromHours(10),
            EndTime = TimeSpan.FromHours(11),
            Status = ReservationStatus.PENDING
        };

        var result = await _reservationsService.SaveReservationAsync(reservation);

        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
        Assert.Equal(reservation.UserId, result.UserId);
    }

    [Fact]
    public async Task GetReservationByIdAsync_ShouldReturnReservation_WhenExists()
    {
        Console.WriteLine("Testing if GetReservationByIdAsync returns a reservation when it exists.");

        var reservation = new Reservation
        {
            UserId = "user123",
            EmployeeId = 2,
            ServiceId = 1,
            ReservationDate = DateTime.UtcNow.Date,
            StartTime = TimeSpan.FromHours(10),
            EndTime = TimeSpan.FromHours(11),
            Status = ReservationStatus.PENDING
        };
        _dbContext.Reservations.Add(reservation);
        await _dbContext.SaveChangesAsync();

        var result = await _reservationsService.GetReservationByIdAsync(reservation.Id);

        Assert.NotNull(result);
        Assert.Equal(reservation.Id, result.Id);
        Assert.Equal(reservation.UserId, result.UserId);
    }

    [Fact]
    public async Task GetAllReservationsAsync_ShouldReturnAllReservations()
    {
        Console.WriteLine("Testing if GetAllReservationsAsync returns all reservations.");

        _dbContext.Reservations.AddRange(new List<Reservation>
        {
            new Reservation { UserId = "user123", EmployeeId = 2, ServiceId = 1, ReservationDate = DateTime.UtcNow.Date, StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10), Status = ReservationStatus.PENDING },
            new Reservation { UserId = "user456", EmployeeId = 3, ServiceId = 2, ReservationDate = DateTime.UtcNow.Date, StartTime = TimeSpan.FromHours(11), EndTime = TimeSpan.FromHours(12), Status = ReservationStatus.COMPLETED }
        });
        await _dbContext.SaveChangesAsync();

        var result = await _reservationsService.GetAllReservationsAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task CancelReservationAsync_ShouldCancelReservation_WhenExists()
    {
        Console.WriteLine("Testing if CancelReservationAsync cancels an existing reservation.");

        var reservation = new Reservation
        {
            UserId = "user123",
            EmployeeId = 2,
            ServiceId = 1,
            ReservationDate = DateTime.UtcNow.Date,
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10),
            Status = ReservationStatus.PENDING
        };
        _dbContext.Reservations.Add(reservation);
        await _dbContext.SaveChangesAsync();

        var result = await _reservationsService.CancelReservationAsync(reservation.Id);

        Assert.NotNull(result);
        Assert.Equal(ReservationStatus.CANCELLED, result.Status);
    }

    public void Dispose()
    {
        _transactionScope.Dispose();
        _dbContext.Dispose();
    }
}
