using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.SupplierMasterRepository
{
    public class SupplierMasterRepository:RepositoryBase<SupplierMaster>,ISupplierMasterRepository
    {
        private readonly ChromeContext _context;
        public SupplierMasterRepository(ChromeContext context):base(context) 
        {
            _context = context;
        }

        public async Task<List<SupplierMaster>> GetAllSupplier(int page, int pageSize)
        {
            var lstSupplier = await _context.SupplierMasters
                                            .OrderBy(x=>x.SupplierCode)
                                            .Skip((page- 1) * pageSize)
                                            .Take(pageSize)
                                            .ToListAsync();
            return lstSupplier;
        }

        public async Task<SupplierMaster> GetSupplierMasterWithSupplierCode(string supplierCode)
        {
            var supplier = await _context.SupplierMasters
                                         .FirstOrDefaultAsync(x=>x.SupplierCode == supplierCode);
            return supplier!;
        }

        public async Task<int> GetTotalSearchCount(string textToSearch)
        {
            var totalSearchCount = await _context.SupplierMasters
                                           .Where(x => x.SupplierCode.Contains(textToSearch)
                                            || x.SupplierName!.Contains(textToSearch)
                                            || x.SupplierPhone!.Contains(textToSearch)
                                            || x.SupplierAddress!.Contains(textToSearch))
                                           .CountAsync();
            return totalSearchCount;
        }

        public async Task<int> GetTotalSupplierCount()
        {
            var totalSupplier = await _context.SupplierMasters
                                              .CountAsync();  
            return totalSupplier;
        }

        public async Task<List<SupplierMaster>> SearchSupplier(string textToSearch, int page, int pageSize)
        {
            var lstSupplier = await _context.SupplierMasters
                                            .Where(x=>x.SupplierCode.Contains(textToSearch)
                                            ||x.SupplierName!.Contains(textToSearch)
                                            ||x.SupplierPhone!.Contains(textToSearch)
                                            ||x.SupplierAddress!.Contains(textToSearch))
                                            .OrderBy(x=>x.SupplierCode)
                                            .Skip((page - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToListAsync();
            return lstSupplier;

        }
    }
}
