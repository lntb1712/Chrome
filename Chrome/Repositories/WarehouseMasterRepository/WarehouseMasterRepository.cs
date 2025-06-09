using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.WarehouseMasterRepository
{
    public class WarehouseMasterRepository: RepositoryBase<WarehouseMaster>, IWarehouseMasterRepository
    {
        private readonly ChromeContext _context;
        public WarehouseMasterRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<WarehouseMaster>> GetWarehouseMasters(int page, int pageSize)
        {
            return await _context.WarehouseMasters
                .OrderBy(x => x.WarehouseCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<int> GetTotalWarehouse()
        {
            return await _context.WarehouseMasters.CountAsync();
        }

        public Task<WarehouseMaster> GetWarehouseMasterWithCode(string warehouseCode)
        {
            var warehouse = _context.WarehouseMasters
                .FirstOrDefaultAsync(row => row.WarehouseCode == warehouseCode);
            return warehouse!;
        }

        public Task<List<WarehouseMaster>> SearchWarehouse(string textToSearch, int page, int pageSize)
        {
            var lstWarehouse = _context.WarehouseMasters
                .Where(x => x.WarehouseCode.Contains(textToSearch) || x.WarehouseName!.Contains(textToSearch) || x.WarehouseAddress!.Contains(textToSearch))
                .OrderBy(x => x.WarehouseCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return lstWarehouse;
        }

        public Task<int> GetTotalSearchCount(string textToSearch)
        {
            var totalCount = _context.WarehouseMasters
                .Where(x => x.WarehouseCode.Contains(textToSearch) || x.WarehouseName!.Contains(textToSearch) || x.WarehouseAddress!.Contains(textToSearch))
                .CountAsync();
            return totalCount;
        }
    }
}
