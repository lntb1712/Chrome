using Chrome.Models;
using Chrome.Repositories.PickListDetailRepository;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Chrome.Repositories.PickListDetailRepository
{
    public class PickListDetailRepository : RepositoryBase<PickListDetail>, IPickListDetailRepository
    {
        private readonly ChromeContext _context;

        public PickListDetailRepository(ChromeContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IQueryable<PickListDetail> GetAllPickListDetailsAsync(string[] warehouseCodes)
        {
            var query = _context.PickListDetails
                                .Include(pd => pd.PickNoNavigation)
                                .ThenInclude(p => p.WarehouseCodeNavigation)
                                .Include(pd => pd.ProductCodeNavigation)
                                .Include(pd => pd.LocationCodeNavigation)
                                .AsQueryable();

            if (warehouseCodes != null && warehouseCodes.Length > 0)
            {
                query = query.Where(pd => warehouseCodes.Contains(pd.PickNoNavigation.WarehouseCode));
            }

            return query;
        }

        public IQueryable<PickListDetail> GetPickListDetailsByPickNoAsync(string pickNo)
        {
            if (string.IsNullOrEmpty(pickNo))
            {
                throw new ArgumentNullException(nameof(pickNo), "Mã pick list không được để trống.");
            }

            var query = _context.PickListDetails
                                .Include(pd => pd.PickNoNavigation)
                                .ThenInclude(p => p.WarehouseCodeNavigation)
                                .Include(pd => pd.ProductCodeNavigation)
                                .Include(pd => pd.LocationCodeNavigation)
                                .Where(pd => pd.PickNo == pickNo);

            return query;
        }

        public IQueryable<PickListDetail> SearchPickListDetailsAsync(string[] warehouseCodes,string pickNo, string textToSearch)
        {
            var query = _context.PickListDetails
                                .Include(pd => pd.PickNoNavigation)
                                .ThenInclude(p => p.WarehouseCodeNavigation)
                                .Include(pd => pd.ProductCodeNavigation)
                                .Include(pd => pd.LocationCodeNavigation)
                                .AsQueryable();

            if (warehouseCodes != null && warehouseCodes.Length > 0)
            {
                query = query.Where(pd => warehouseCodes.Contains(pd.PickNoNavigation.WarehouseCode));
            }

            if (!string.IsNullOrEmpty(textToSearch))
            {
                query = query.Where(pd => pd.PickNo==pickNo &&(pd.PickNo.Contains(textToSearch)
                                      || pd.ProductCode.Contains(textToSearch)
                                      || pd.LotNo!.Contains(textToSearch)
                                      || pd.LocationCode!.Contains(textToSearch)
                                      || pd.PickNoNavigation.PickNo.Contains(textToSearch)
                                      || pd.ProductCodeNavigation.ProductName!.Contains(textToSearch)
                                      || pd.LocationCodeNavigation!.LocationName!.Contains(textToSearch)));
            }

            return query;
        }
    }
}