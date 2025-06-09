using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.StorageProductRepository
{
    public class StorageProductRepository:RepositoryBase<StorageProduct>,IStorageProductRepository
    {
        private readonly ChromeContext _context;
        public StorageProductRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<StorageProduct>> GetAllStorageProducts(int page, int pageSize)
        {
            return await _context.StorageProducts
                .Include(x=>x.ProductCodeNavigation)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<StorageProduct> GetStorageProductWithCode(string storageProductCode)
        {
            var storageProduct= await _context.StorageProducts
                .Include(x => x.ProductCodeNavigation)
                .FirstOrDefaultAsync(x => x.StorageProductId == storageProductCode);
            return storageProduct!;
        }

        public async Task<int> GetTotalSearchCount(string textToSearch)
        {
            return await _context.StorageProducts
                .Where(x => x.StorageProductId.Contains(textToSearch) 
                || x.StorageProductName!.Contains(textToSearch)
                || x.ProductCodeNavigation!.ProductName!.Contains(textToSearch))
                .CountAsync();
        }

        public async Task<int> GetTotalStorageProductCount()
        {
            return await _context.StorageProducts
                .Include(x => x.ProductCodeNavigation)
                .CountAsync();
        }

        public Task<List<StorageProduct>> SearchStorageProducts(string textToSearch, int page, int pageSize)
        {
            return _context.StorageProducts
                .Include(x => x.ProductCodeNavigation)
                .Where(x => x.StorageProductId.Contains(textToSearch) 
                || x.StorageProductName!.Contains(textToSearch)
                || x.ProductCodeNavigation!.ProductName!.Contains(textToSearch))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
