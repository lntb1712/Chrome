using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.PutAwayRulesRepository
{
    public class PutAwayRulesRepository : RepositoryBase<PutAwayRule>, IPutAwayRulesRepository
    {
        private readonly ChromeContext _context;
        public PutAwayRulesRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<PutAwayRule>> GetAllPutAwayRules(int page, int pageSize)
        {
            var lstPutAwayRules = await _context.PutAwayRules
                .Include(x=>x.WarehouseToApplyNavigation)
                .Include(x=>x.StorageProduct)
                .Include(x=>x.ProductCodeNavigation)
                .Include(x=>x.LocationCodeNavigation)
                .OrderBy(x => x.PutAwaysRuleCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return lstPutAwayRules;
        }

        public async Task<PutAwayRule> GetPutAwayRuleWithCode(string putAwayRuleCode)
        {
            var putAwayRule = await _context.PutAwayRules
                .Include(x => x.WarehouseToApplyNavigation)
                .Include(x => x.StorageProduct)
                .Include(x => x.ProductCodeNavigation)
                .Include(x => x.LocationCodeNavigation)
                .FirstOrDefaultAsync(x => x.PutAwaysRuleCode == putAwayRuleCode);
            return putAwayRule!;

        }

        public async Task<int> GetTotalPutAwayRuleCount()
        {
            return await _context.PutAwayRules
                .Include(x => x.WarehouseToApplyNavigation)
                .Include(x => x.StorageProduct)
                .Include(x => x.ProductCodeNavigation)
                .Include(x => x.LocationCodeNavigation)
                .CountAsync();
        }

        public async Task<int> GetTotalSearchCount(string textToSearch)
        {
            return await _context.PutAwayRules
                                .Include(x => x.WarehouseToApplyNavigation)
                                .Include(x => x.StorageProduct)
                                .Include(x => x.ProductCodeNavigation)
                                .Include(x => x.LocationCodeNavigation)
                                .Where(x => x.PutAwaysRuleCode.Contains(textToSearch)
                                || x.WarehouseToApplyNavigation!.WarehouseName!.Contains(textToSearch)
                                || x.StorageProduct!.StorageProductName!.Contains(textToSearch)
                                || x.ProductCodeNavigation!.ProductName!.Contains(textToSearch)
                                || x.LocationCodeNavigation!.LocationName!.Contains(textToSearch)).CountAsync();
        }

        public async Task<List<PutAwayRule>> SearchPutAwayRules(string textToSearch, int page, int pageSize)
        {
            return await _context.PutAwayRules
                .Include(x => x.WarehouseToApplyNavigation)
                .Include(x => x.StorageProduct)
                .Include(x => x.ProductCodeNavigation)
                .Include(x => x.LocationCodeNavigation)
                .Where(x => x.PutAwaysRuleCode.Contains(textToSearch) 
                            || x.WarehouseToApplyNavigation!.WarehouseName!.Contains(textToSearch)
                            || x.StorageProduct!.StorageProductName!.Contains(textToSearch)
                            || x.ProductCodeNavigation!.ProductName!.Contains(textToSearch)
                            || x.LocationCodeNavigation!.LocationName!.Contains(textToSearch))
                .OrderBy(x => x.PutAwaysRuleCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
