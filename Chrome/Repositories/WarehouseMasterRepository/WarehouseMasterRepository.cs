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
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<int> GetTotalWarehouse()
        {
            return await _context.WarehouseMasters.CountAsync();
        }
    }
}
