using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class ReservationsService : IReservationsService
{
    private readonly IReservationsRepository _reservationsRepository;

    public ReservationsService(IReservationsRepository reservationsRepository)
    {
        _reservationsRepository = reservationsRepository;
    }

    public async Task MarkReservationsAsCompletedAsync()
    {
        await _reservationsRepository.UpdateCompletedReservationsAsync();
    }

    public async Task<Reservation> GetReservationByIdAsync(int reservationId)
    {
        var reservation = await _reservationsRepository.GetReservationByIdAsync(reservationId);
        if (reservation == null)
        {
            throw new ArgumentException($"Reservation not found with id: {reservationId}");
        }
        return reservation;
    }

    public async Task<List<Reservation>> GetAllReservationsAsync()
    {
        return await _reservationsRepository.GetAllReservationsAsync();
    }

    public async Task<List<Reservation>> GetAllActiveReservationsAsync(string userId)
    {
        return await _reservationsRepository.GetAllActiveReservationsAsync(userId);
    }

    public async Task<List<Reservation>> GetAllHistoryReservationsAsync(string userId)
    {
        return await _reservationsRepository.GetAllHistoryReservationsAsync(userId);
    }

    public async Task<Reservation> CancelReservationAsync(int reservationId)
    {
        return await _reservationsRepository.CancelReservationAsync(reservationId);
    }

    public async Task<Reservation> SaveReservationAsync(Reservation reservation)
    {
        return await _reservationsRepository.SaveReservationAsync(reservation);
    }
}
