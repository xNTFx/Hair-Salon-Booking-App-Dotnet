using System.Collections.Generic;
using System.Threading.Tasks;

public interface IReservationsService
{
    Task MarkReservationsAsCompletedAsync();
    Task<Reservation> GetReservationByIdAsync(int reservationId);
    Task<List<Reservation>> GetAllReservationsAsync();
    Task<List<Reservation>> GetAllActiveReservationsAsync(string userId);
    Task<List<Reservation>> GetAllHistoryReservationsAsync(string userId);
    Task<Reservation> CancelReservationAsync(int reservationId);
    Task<Reservation> SaveReservationAsync(Reservation reservation);
}
