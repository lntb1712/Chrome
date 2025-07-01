using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.ReservationDetailRepository
{
    public class ReservationDetailRepository : RepositoryBase<ReservationDetail>, IReservationDetailRepository
    {
        private readonly ChromeContext _context;

        public ReservationDetailRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<ReservationDetail> GetAllReservationDetailsAsync(string reservationCode)
        {
            return _context.ReservationDetails
                .Include(x => x.ReservationCodeNavigation)
                .Where(x => x.ReservationCode == reservationCode);
        }

        public async Task<ReservationDetail> GetReservationDetailByIdAsync(int id)
        {
            return await _context.ReservationDetails
                 .Include(x => x.ReservationCodeNavigation)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new InvalidOperationException($"ReservationDetail with ID {id} not found.");
        }

        public IQueryable<ReservationDetail> SearchReservationDetailsAsync(string reservationCode, string textToSearch)
        {
            return _context.ReservationDetails
                 .Include(x => x.ReservationCodeNavigation)
                .Where(x => x.ReservationCode == reservationCode
                    && (x.ProductCode!.Contains(textToSearch)

                        || x.LotNo!.Contains(textToSearch)
                        || x.LocationCode!.Contains(textToSearch)));
        }
    }
}