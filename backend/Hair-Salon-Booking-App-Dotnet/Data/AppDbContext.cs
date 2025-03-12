using Microsoft.EntityFrameworkCore;
using System;

public class AppDbContext : DbContext
{
    public DbSet<Service> Services { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<AvailableHours> AvailableHours { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Service>().ToTable("services");
        modelBuilder.Entity<Employee>().ToTable("employees");
        modelBuilder.Entity<AvailableHours>().ToTable("available_hours");
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<Reservation>().ToTable("reservations");

        // Converter for ReservationStatus to save enum as string
        modelBuilder.Entity<Reservation>()
            .Property(r => r.Status)
            .HasConversion(
                v => v.ToString(),
                v => (ReservationStatus)Enum.Parse(typeof(ReservationStatus), v) 
            );
    }
}
