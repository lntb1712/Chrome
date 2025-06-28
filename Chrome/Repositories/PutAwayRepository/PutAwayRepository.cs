using Chrome.Models;
using Chrome.Repositories.PutawayRepository;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Chrome.Repositories.PutAwayRepository
{
    public class PutAwayRepository : RepositoryBase<PutAway>, IPutAwayRepository
    {
        private readonly ChromeContext _context;

        public PutAwayRepository(ChromeContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IQueryable<PutAway> GetAllPutAwayAsync(string[] warehouseCodes)
        {
            var query = _context.PutAways
                               .Include(p => p.OrderTypeCodeNavigation)
                               .Include(p=> p.LocationCodeNavigation)
                               .Include(p=>p.ResponsibleNavigation)
                               .Include(p=>p.Status)
                               .AsQueryable();

            if (warehouseCodes != null && warehouseCodes.Length > 0)
            {
                query = query.Where(p => warehouseCodes.Contains(p.LocationCodeNavigation!.WarehouseCode));
            }

            return query;
        }

        public IQueryable<PutAway> GetAllPutAwayWithStatus(string[] warehouseCodes, int statusId)
        {
            var query = _context.PutAways
                                .Include(p => p.OrderTypeCodeNavigation)
                               .Include(p => p.LocationCodeNavigation)
                               .Include(p => p.ResponsibleNavigation)
                               .Include(p => p.Status)
                               .Where(p => p.StatusId == statusId)
                               .AsQueryable();

            if (warehouseCodes != null && warehouseCodes.Length > 0)
            {
                query = query.Where(p => warehouseCodes.Contains(p.LocationCodeNavigation!.WarehouseCode));
            }

            return query;
        }

        public IQueryable<PutAway> SearchPutAwayAsync(string[] warehouseCodes, string textToSearch)
        {
            var query = _context.PutAways
                                .Include(p => p.OrderTypeCodeNavigation)
                               .Include(p => p.LocationCodeNavigation)
                               .Include(p => p.ResponsibleNavigation)
                               .Include(p => p.Status)
                               .AsQueryable();

            if (warehouseCodes != null && warehouseCodes.Length > 0)
            {
                query = query.Where(p => warehouseCodes.Contains(p.LocationCodeNavigation!.WarehouseCode));
            }

            if (!string.IsNullOrEmpty(textToSearch))
            {
                query = query.Where(p => p.PutAwayCode.Contains(textToSearch)
                                      || p.LocationCodeNavigation!.WarehouseCodeNavigation!.WarehouseName!.Contains(textToSearch));
            }

            return query;
        }

        public async Task<PutAway> GetPutAwayCodeAsync(string PutAwayCode)
        {
            if (string.IsNullOrEmpty(PutAwayCode))
            {
                throw new ArgumentNullException(nameof(PutAwayCode), "Mã putaway không được để trống.");
            }

            var putaway = await _context.PutAways
                                       .Include(p => p.OrderTypeCodeNavigation)
                               .Include(p => p.LocationCodeNavigation)
                               .Include(p => p.ResponsibleNavigation)
                               .Include(p => p.Status)
                                       .FirstOrDefaultAsync(p => p.PutAwayCode == PutAwayCode);

          

            return putaway!;
        }
        public async Task<PutAway> GetPutAwayContainsCodeAsync(string orderCode)
        {
            if (string.IsNullOrEmpty(orderCode))
            {
                throw new ArgumentNullException(nameof(orderCode), "Mã lệnh không được để trống.");
            }

            var putaway = await _context.PutAways
                                       .Include(p => p.OrderTypeCodeNavigation)
                                       .Include(p => p.LocationCodeNavigation)
                                       .Include(p => p.ResponsibleNavigation)
                                       .Include(p => p.Status)
                                       .FirstOrDefaultAsync(p => p.PutAwayCode.Contains(orderCode));



            return putaway!;
        }

        public async Task<PutAway> GetPutAwayContainsMovement(string movementCode)
        {
            if (string.IsNullOrEmpty(movementCode))
            {
                throw new ArgumentNullException(nameof(movementCode), "Mã putaway không được để trống.");
            }

            var putaway = await _context.PutAways
                                       .Include(p => p.OrderTypeCodeNavigation)
                               .Include(p => p.LocationCodeNavigation)
                               .Include(p => p.ResponsibleNavigation)
                               .Include(p => p.Status)
                                       .FirstOrDefaultAsync(p => p.PutAwayCode.Contains (movementCode));



            return putaway!;
        }
    }
}