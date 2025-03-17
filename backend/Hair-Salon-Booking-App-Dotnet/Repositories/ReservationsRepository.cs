using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ReservationsRepository : IReservationsRepository
{
    private readonly AppDbContext _context;

    public ReservationsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Reservation>> GetAllReservationsAsync()
    {
        return await _context.Reservations
            .Include(r => r.Service)
            .Include(r => r.Employee)
            .ToListAsync();
    }

    public async Task<List<Reservation>> GetAllActiveReservationsAsync(string userId)
    {
        return await _context.Reservations
            .Include(r => r.Service)
            .Include(r => r.Employee)
            .Where(r => r.UserId == userId && r.Status == ReservationStatus.PENDING)
            .ToListAsync();
    }

    public async Task<List<Reservation>> GetAllHistoryReservationsAsync(string userId)
    {
        return await _context.Reservations
            .Include(r => r.Service)
            .Include(r => r.Employee)
            .Where(r => r.UserId == userId &&
                        (r.Status == ReservationStatus.CANCELLED || r.Status == ReservationStatus.COMPLETED))
            .ToListAsync();
    }

    public async Task<Reservation> GetReservationByIdAsync(int reservationId)
    {
        return await _context.Reservations
            .Include(r => r.Service)
            .Include(r => r.Employee)
            .FirstOrDefaultAsync(r => r.Id == reservationId);
    }

    public async Task<Reservation> CancelReservationAsync(int reservationId)
    {
        var reservation = await _context.Reservations.FindAsync(reservationId);
        if (reservation != null)
        {
            reservation.Status = ReservationStatus.CANCELLED;
            await _context.SaveChangesAsync();
        }
        return reservation;
    }

    public async Task<Reservation> SaveReservationAsync(Reservation reservation)
    {
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();
        return reservation;
    }

public async Task UpdateCompletedReservationsAsync()
{
    var now = DateTime.UtcNow;
    
    var reservationsToUpdate = await _context.Reservations
        .Where(r => r.Status == ReservationStatus.PENDING)
        .ToListAsync();

    foreach (var reservation in reservationsToUpdate)
    {
        var reservationDateTime = reservation.ReservationDate.Add(reservation.EndTime);
        if (reservationDateTime < now)
        {
            reservation.Status = ReservationStatus.COMPLETED;
        }
    }

    await _context.SaveChangesAsync();
}
}
