using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.ProductCustomerRepository
{
    public class ProductCustomerRepository:RepositoryBase<CustomerProduct>, IProductCustomerRepository
    {
        private readonly ChromeContext _context;
        
        public ProductCustomerRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<CustomerProduct>> GetAllCustomerProducts(string productCode,int page, int pageSize)
        {
            return await _context.CustomerProducts
                .Include(cp => cp.CustomerCodeNavigation) // Include related CustomerMaster data
                .Include(cp => cp.ProductCodeNavigation) // Include related ProductMaster data
                .Where(cp => cp.ProductCode == productCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<int> GetTotalCustomerProductCount(string productCode)
        {
            return await _context.CustomerProducts
                                 .Include(cp => cp.CustomerCodeNavigation) // Include related CustomerMaster data
                                 .Include(cp => cp.ProductCodeNavigation) // Include related ProductMaster data
                                 .Where(cp => cp.ProductCode == productCode)
                                 .CountAsync();
        }
        public async Task<CustomerProduct> GetAllCustomerProductsByCode(string productCode,string customerCode)
        {
            var customerProduct = await _context.CustomerProducts
                .Include(cp => cp.CustomerCodeNavigation) // Include related CustomerMaster data
                .Include(cp => cp.ProductCodeNavigation) // Include related ProductMaster data
                .FirstOrDefaultAsync(cp => cp.ProductCode == productCode && cp.CustomerCode == customerCode);
            return customerProduct!;
        }

    }
}
