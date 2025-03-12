using System.Collections.Generic;
using System.Threading.Tasks;

public interface IReservationsRepository
{
    Task<List<Reservation>> GetAllReservationsAsync();
    Task<List<Reservation>> GetAllActiveReservationsAsync(string userId);
    Task<List<Reservation>> GetAllHistoryReservationsAsync(string userId);
    Task<Reservation> GetReservationByIdAsync(int reservationId);
    Task<Reservation> CancelReservationAsync(int reservationId);
    Task<Reservation> SaveReservationAsync(Reservation reservation);
    Task UpdateCompletedReservationsAsync();
}
