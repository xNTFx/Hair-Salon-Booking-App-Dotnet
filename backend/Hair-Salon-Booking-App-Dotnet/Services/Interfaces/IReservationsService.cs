using System.Collections.Generic;
using System.Threading.Tasks;

public interface IReservationsService
{
    // marks reservations as completed asynchronously
    Task MarkReservationsAsCompletedAsync();

    // retrieves a reservation by its ID
    Task<Reservation> GetReservationByIdAsync(int reservationId);

    // retrieves all reservations
    Task<List<Reservation>> GetAllReservationsAsync();

    // retrieves all active reservations for a specific user
    Task<List<Reservation>> GetAllActiveReservationsAsync(string userId);

    // retrieves reservation history for a specific user
    Task<List<Reservation>> GetAllHistoryReservationsAsync(string userId);

    // cancels a reservation by its ID
    Task<Reservation> CancelReservationAsync(int reservationId);

    // saves a new reservation
    Task<Reservation> SaveReservationAsync(Reservation reservation);
}
