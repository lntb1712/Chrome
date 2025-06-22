using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.ReservationRepository
{
    public interface IReservationRepository:IRepositoryBase<Reservation>
    {
        IQueryable<Reservation> GetAllReservationsAsync(string[] warehouseCodes);
        IQueryable<Reservation> GetAllReservationsWithStatus(string[] warehouseCodes, int statusId);
        IQueryable<Reservation> SearchReservationsAsync(string[] warehouseCodes, string textToSearch);
        Task<Reservation> GetReservationWithCode(string reservationCode);
    }
}
