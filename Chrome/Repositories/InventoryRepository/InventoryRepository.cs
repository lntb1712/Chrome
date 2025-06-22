using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.InventoryRepository
{
    public class InventoryRepository : RepositoryBase<Inventory>, IInventoryRepository
    {
        private readonly ChromeContext _context;
        public InventoryRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Inventory> GetInventoryWithCode(string warehouseCode, string locationCode, string productCode, string lotNo)
        {
            var inventory= await _context.Inventories
                                 .Include(x => x.WarehouseCodeNavigation)
                                 .Include(x => x.LocationCodeNavigation)
                                 .Include(x => x.ProductCodeNavigation)
                                 .ThenInclude(x => x.Category)
                                 .FirstOrDefaultAsync(x => x.WarehouseCode == warehouseCode
                                                           && x.ProductCode == productCode
                                                           && x.LocationCode == locationCode
                                                           && x.Lotno == lotNo);
            return inventory!;
        }

        public IQueryable<Inventory> GetInventories(string[] warehouseCodes)
        {
            return _context.Inventories
                           .Include(x => x.WarehouseCodeNavigation)
                           .Include(x => x.LocationCodeNavigation)
                           .Include(x => x.ProductCodeNavigation)
                           .ThenInclude(x => x.Category)
                           .Where(x => warehouseCodes.Contains(x.WarehouseCode));
        }

        public IQueryable<Inventory> GetInventoriesByCategoryIds(string[] warehouseCodes, string[] categoryIds)
        {
            return _context.Inventories
                           .Include(x => x.WarehouseCodeNavigation)
                           .Include(x => x.LocationCodeNavigation)
                           .Include(x => x.ProductCodeNavigation)
                           .ThenInclude(x => x.Category)
                           .Where(x => warehouseCodes.Contains(x.WarehouseCode)
                                    && categoryIds.Contains(x.ProductCodeNavigation.CategoryId));
        }

        public IQueryable<Inventory> SearchProductInventories(string[] warehouseCodes, string textToSearch)
        {
            return _context.Inventories
                           .Include(x => x.WarehouseCodeNavigation)
                           .Include(x => x.LocationCodeNavigation)
                           .Include(x => x.ProductCodeNavigation)
                           .ThenInclude(x => x.Category)
                           .Where(x => warehouseCodes.Contains(x.WarehouseCode)
                                    && (x.ProductCode.Contains(textToSearch)
                                        || x.ProductCodeNavigation.ProductName!.Contains(textToSearch)
                                        || x.ProductCodeNavigation.CategoryId!.Contains(textToSearch)
                                        || x.ProductCodeNavigation.Category!.CategoryName!.Contains(textToSearch)));
        }

        public IQueryable<Inventory> GetInventoryByProductCodeAsync(string productCode, string warehouseCode)
        {
            return _context.Inventories
                           .Include(x => x.WarehouseCodeNavigation)
                           .Include(x => x.LocationCodeNavigation)
                           .Include(x => x.ProductCodeNavigation)
                           .ThenInclude(x => x.Category)
                           .Where(x => x.ProductCode == productCode && x.WarehouseCode == warehouseCode);
        }
    }
}
