using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Chrome.Repositories.PutAwayDetailRepository
{
    public class PutAwayDetailRepository : RepositoryBase<PutAwayDetail>, IPutAwayDetailRepository
    {
        private readonly ChromeContext _context;

        public PutAwayDetailRepository(ChromeContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IQueryable<PutAwayDetail> GetAllPutAwayDetailsAsync(string[] warehouseCodes)
        {
            var query = _context.PutAwayDetails
                                 .Include(pd => pd.PutAwayCodeNavigation)
                               .ThenInclude(p => p.LocationCodeNavigation)
                               .ThenInclude(p => p!.WarehouseCodeNavigation)
                               .Include(pd => pd.ProductCodeNavigation)
                               .AsQueryable();

            if (warehouseCodes != null && warehouseCodes.Length > 0)
            {
                query = query.Where(pd => warehouseCodes.Contains(pd.PutAwayCodeNavigation.LocationCodeNavigation!.WarehouseCode));
            }

            return query;
        }

        public IQueryable<PutAwayDetail> GetPutAwayDetailsByPutawayNoAsync(string putawayNo)
        {
            if (string.IsNullOrEmpty(putawayNo))
            {
                throw new ArgumentNullException(nameof(putawayNo), "Mã putaway không được để trống.");
            }

            var query = _context.PutAwayDetails
                               .Include(pd => pd.PutAwayCodeNavigation)
                               .ThenInclude(p => p.LocationCodeNavigation)
                               .ThenInclude(p=>p!.WarehouseCodeNavigation)
                               .Include(pd => pd.ProductCodeNavigation)
                               .Where(pd => pd.PutAwayCode == putawayNo);

            return query;
        }

        public IQueryable<PutAwayDetail> SearchPutAwayDetailsAsync(string[] warehouseCodes, string putawayNo, string textToSearch)
        {
            var query = _context.PutAwayDetails
                                 .Include(pd => pd.PutAwayCodeNavigation)
                               .ThenInclude(p => p.LocationCodeNavigation)
                               .ThenInclude(p => p!.WarehouseCodeNavigation)
                               .Include(pd => pd.ProductCodeNavigation)
                               .AsQueryable();

            if (warehouseCodes != null && warehouseCodes.Length > 0)
            {
                query = query.Where(pd => warehouseCodes.Contains(pd.PutAwayCodeNavigation.LocationCodeNavigation!.WarehouseCode));
            }

            if (!string.IsNullOrEmpty(textToSearch))
            {
                query = query.Where(pd => pd.PutAwayCode == putawayNo && (
                    pd.PutAwayCode.Contains(textToSearch)
                    || pd.ProductCode.Contains(textToSearch)
                    || pd.LotNo!.Contains(textToSearch)
                    || pd.Demand.ToString()!.Contains(textToSearch)
                    || pd.Quantity.ToString()!.Contains(textToSearch)
                    || pd.PutAwayCodeNavigation.PutAwayCode!.Contains(textToSearch)
                    || pd.ProductCodeNavigation.ProductName!.Contains(textToSearch)
                    || pd.PutAwayCodeNavigation.LocationCodeNavigation!.LocationName!.Contains(textToSearch)));
            }

            return query;
        }
    }
}