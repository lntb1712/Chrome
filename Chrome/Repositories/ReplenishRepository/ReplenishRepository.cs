using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.ReplenishRepository
{
    public class ReplenishRepository:RepositoryBase<Replenish>, IReplenishRepository
    {
        private readonly ChromeContext _context;
        public ReplenishRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Replenish> GetAllReplenishAsync(string warehouseCode)
        {
            var query = _context.Replenishes
                                .Include(x=>x.ProductCodeNavigation)
                                .ThenInclude(x=>x.Inventories)
                                .Include(x=>x.WarehouseCodeNavigation)
                                .Where(r => r.WarehouseCode == warehouseCode)
                                .AsQueryable(); 
            return query;
        }

        public async Task<Replenish> GetReplenishByCode(string productCode, string warehouseCode)
        {
            var query = await _context.Replenishes
                                .Include(x => x.ProductCodeNavigation)
                                .ThenInclude(x => x.Inventories)
                                .Include(x => x.WarehouseCodeNavigation)
                                .FirstOrDefaultAsync(r => r.ProductCode == productCode && r.WarehouseCode == warehouseCode);
            return query!;
        }

        public IQueryable<Replenish> SearchReplenishAsync(string warehouseCode, string textToSearch)
        {
            var query = _context.Replenishes
                                 .Include(x => x.ProductCodeNavigation)
                                 .ThenInclude(x => x.Inventories)
                                 .Include(x => x.WarehouseCodeNavigation)
                                 .Where(r => r.WarehouseCode == warehouseCode&&
                                 (r.ProductCode.Contains(textToSearch)
                                 ||r.WarehouseCode.Contains(textToSearch)))
                                 .AsQueryable();
            return query;
        }
    }
}
