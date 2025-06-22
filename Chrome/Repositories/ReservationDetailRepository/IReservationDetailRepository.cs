using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.ReservationDetailRepository
{
    public interface IReservationDetailRepository : IRepositoryBase<ReservationDetail>
    {
        IQueryable<ReservationDetail> GetAllReservationDetailsAsync(string reservationCode);
        Task<ReservationDetail> GetReservationDetailByIdAsync(int id);
        IQueryable<ReservationDetail> SearchReservationDetailsAsync(string reservationCode, string textToSearch);
    }
}
