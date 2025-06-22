using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
                .Include(x => x.ProductCodeNavigation)
                .Include(x => x.LocationCodeNavigation)
                .Where(x => x.ReservationCode == reservationCode);
        }

        public async Task<ReservationDetail> GetReservationDetailByIdAsync(int id)
        {
            return await _context.ReservationDetails
                 .Include(x => x.ReservationCodeNavigation)
                .Include(x => x.ProductCodeNavigation)
                .Include(x => x.LocationCodeNavigation)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new InvalidOperationException($"ReservationDetail with ID {id} not found.");
        }

        public IQueryable<ReservationDetail> SearchReservationDetailsAsync(string reservationCode, string textToSearch)
        {
            return _context.ReservationDetails
                 .Include(x => x.ReservationCodeNavigation)
                .Include(x => x.ProductCodeNavigation)
                .Include(x => x.LocationCodeNavigation)
                .Where(x => x.ReservationCode == reservationCode
                    && (x.ProductCode!.Contains(textToSearch)
                        || x.ProductCodeNavigation!.ProductName!.Contains(textToSearch)
                        || x.Lotno!.Contains(textToSearch)
                        || x.LocationCode!.Contains(textToSearch)
                        || x.LocationCodeNavigation!.LocationName!.Contains(textToSearch)));
        }
    }
}