using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.LocationMasterRepository
{
    public class LocationMasterRepository : RepositoryBase<LocationMaster>, ILocationMasterRepository
    {
        private readonly ChromeContext _context;
        public LocationMasterRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<LocationMaster>> GetAllLocationMaster(string warehouseCode, int page, int pageSize)
        {
            var lstLocationMaster = await _context.LocationMasters
                .Include(x=>x.WarehouseCodeNavigation)
                .Include(x=>x.StorageProduct)
                .Include(x=>x.Inventories)
                .Where(x=>x.WarehouseCode==warehouseCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return lstLocationMaster;
        }

        public async Task<LocationMaster> GetLocationMasterWithCode(string warehouseCode, string locationCode)
        {
            var locationMaster =  await _context.LocationMasters
                .Include(x => x.WarehouseCodeNavigation)
                .Include(x => x.StorageProduct)
                .Include(x => x.Inventories)
                .FirstOrDefaultAsync(x => x.LocationCode == locationCode && x.WarehouseCode == warehouseCode);
            return locationMaster!;
        }

        public async Task<int> GetTotalLocationMasterCount(string warehouseCode)
        {
            return await _context.LocationMasters
                .Include(x => x.WarehouseCodeNavigation)
                .Include(x => x.StorageProduct)
                .Include(x=>x.Inventories)
                .Where(x=>x.WarehouseCode==warehouseCode)
                .CountAsync();
        }
    }
}
