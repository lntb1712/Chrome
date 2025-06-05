using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.ProductSupplierRepository
{
    public class ProductSupplierRepository:RepositoryBase<ProductSupplier>, IProductSupplierRepository
    {
        private readonly ChromeContext _context;
        public ProductSupplierRepository(ChromeContext context):base(context) 
        {
            _context = context;
        }

        public async Task<List<ProductSupplier>> GetAllProductSupplierWithProductCode(string productCode, int page, int pageSize)
        {
            var lstProducts = await _context.ProductSuppliers
                                            .Include(x=>x.ProductCodeNavigation)
                                            .Include(x=>x.SupplierCodeNavigation)
                                            .Where(x=>x.ProductCode == productCode)
                                            .OrderBy(x=>x.SupplierCode)
                                            .Skip((page-1)*pageSize)
                                            .Take(pageSize)
                                            .ToListAsync();
            return lstProducts;
        }

        public async Task<ProductSupplier> GetProductSupplierWithProductCodeAndSupplierCode(string productCode, string supplierCode)
        {
            var productSupplier = await _context.ProductSuppliers
                                                .Include (x=>x.ProductCodeNavigation)
                                                .Include (x=>x.SupplierCodeNavigation)
                                                .FirstOrDefaultAsync(x=>x.ProductCode==productCode && x.SupplierCode == supplierCode);
            return productSupplier!; 
        }

        public async Task<int> GetTotalProductSupplierProductCodeCount(string productCode)
        {
            var totalProductSupplier = await _context.ProductSuppliers
                                                     .Include(x => x.ProductCodeNavigation)
                                                     .Include(x => x.SupplierCodeNavigation)
                                                     .Where(x => x.ProductCode == productCode)
                                                     .CountAsync();

            return totalProductSupplier;
        }
    }
}
