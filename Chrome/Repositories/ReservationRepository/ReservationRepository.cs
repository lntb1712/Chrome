using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Chrome.Repositories.ReservationRepository
{
    public class ReservationRepository : RepositoryBase<Reservation>, IReservationRepository
    {
        private readonly ChromeContext _context;

        public ReservationRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Reservation> GetAllReservationsAsync(string[] warehouseCodes)
        {
            var reservations = _context.Reservations
                .Include(x=>x.ReservationDetails)
                .Include(x => x.OrderTypeCodeNavigation)
                .Include(x => x.WarehouseCodeNavigation)
                .Include(x => x.Status)
                .Where(x => warehouseCodes.Contains(x.WarehouseCode));

            return reservations;
        }

        public IQueryable<Reservation> GetAllReservationsWithStatus(string[] warehouseCodes, int statusId)
        {
            var reservations = _context.Reservations
                .Include(x => x.OrderTypeCodeNavigation)
                .Include(x => x.WarehouseCodeNavigation)
                .Include(x => x.Status)
                .Where(x => warehouseCodes.Contains(x.WarehouseCode) && x.StatusId == statusId);

            return reservations;
        }

        public async Task<Reservation> GetReservationWithCode(string reservationCode)
        {
            var reservation = await _context.Reservations
                .Include(x => x.OrderTypeCodeNavigation)
                .Include(x => x.WarehouseCodeNavigation)
                .Include(x => x.Status)
                .FirstOrDefaultAsync(x => x.ReservationCode == reservationCode);
            return reservation ?? throw new InvalidOperationException($"Reservation with code {reservationCode} not found.");
        }

        public IQueryable<Reservation> SearchReservationsAsync(string[] warehouseCodes, string textToSearch)
        {
            var reservations = _context.Reservations
                .Include(x => x.OrderTypeCodeNavigation)
                .Include(x => x.WarehouseCodeNavigation)
                .Include(x => x.Status)
                .Where(x => warehouseCodes.Contains(x.WarehouseCode)
                    && (x.ReservationCode.Contains(textToSearch)
                        || x.OrderTypeCode!.Contains(textToSearch)
                        || x.OrderTypeCodeNavigation!.OrderTypeName!.Contains(textToSearch)
                        || x.OrderId!.Contains(textToSearch)
                        || x.WarehouseCode!.Contains(textToSearch)
                        || x.WarehouseCodeNavigation!.WarehouseName!.Contains(textToSearch)
                        || x.ReservationDate.ToString()!.Contains(textToSearch)));

            return reservations;
        }
    }
}