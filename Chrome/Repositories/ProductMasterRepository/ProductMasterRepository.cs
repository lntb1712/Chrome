using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.ProductMasterRepository
{
    public class ProductMasterRepository : RepositoryBase<ProductMaster>, IProductMasterRepository
    {
        private readonly ChromeContext _context;
        public ProductMasterRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ProductMaster>> GetAllProduct(int page, int pageSize)
        {
            var lstProduct = await _context.ProductMasters
                                           .Include(x => x.Category)
                                           .Include(x=>x.Inventories)
                                           .OrderBy(x => x.ProductCode)
                                           .Skip((page - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToListAsync();
            return lstProduct;
        }

        public Task<List<ProductMaster>> GetAllProductWithCategoryID(string categoryId, int page, int pageSize)
        {
            var lstProduct = _context.ProductMasters
                                     .Include(x => x.Category)
                                     .Include(x => x.Inventories)
                                     .Where(x => x.CategoryId == categoryId)
                                     .OrderBy(x => x.ProductCode)
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();
            return lstProduct;
        }

        public Task<ProductMaster> GetProductMasterWithProductCode(string productCode)
        {
            var product = _context.ProductMasters
                                 .Include(x => x.Category)
                                 .Include(x => x.Inventories)
                                 .FirstOrDefaultAsync(row => row.ProductCode == productCode);
            return product!;
        }

        public Task<int> GetTotalProductCount()
        {
            var totalCount = _context.ProductMasters.Include(x => x.Category).Include(x => x.Inventories).CountAsync();
            return totalCount;
        }

        public Task<int> GetTotalProductWithCategoryIDCount(string categoryId)
        {
            var totalCount = _context.ProductMasters
                                     .Include(x => x.Category)
                                     .Include(x => x.Inventories)
                                     .Where(x => x.CategoryId == categoryId)
                                     .CountAsync();
            return totalCount;
        }

        public Task<int> GetTotalSearchCount(string textToSearch)
        {
            var totalCount = _context.ProductMasters
                                     .Include(x => x.Category)
                                     .Include(x => x.Inventories)
                                     .Where(x => x.ProductCode.Contains(textToSearch) || x.ProductName!.Contains(textToSearch) || x.ProductDescription!.Contains(textToSearch))
                                     .CountAsync();
            return totalCount;
        }

        public Task<List<ProductMaster>> SearchProduct(string textToSearch, int page, int pageSize)
        {
            var lstProduct = _context.ProductMasters
                                     .Include(x => x.Category)
                                     .Include(x => x.Inventories)
                                     .Where(x => x.ProductCode.Contains(textToSearch) || x.ProductName!.Contains(textToSearch) || x.ProductDescription!.Contains(textToSearch))
                                     .OrderBy(x => x.ProductCode)
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();
            return lstProduct;
        }
    }
}
