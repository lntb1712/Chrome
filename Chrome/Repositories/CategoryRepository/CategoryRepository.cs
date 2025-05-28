using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.CategoryRepository
{
    public class CategoryRepository:RepositoryBase<Category>, ICategoryRepository
    {
        private readonly ChromeContext _context;
        public CategoryRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<Category>> GetAllCategories()
        {
            var lstCategory = await _context.Categories
                                            .ToListAsync();
            return lstCategory;
        }
        public async Task<List<Category>> GetTotalProductInCategory()
        {
            var category = await _context.Categories
                                         .Include(x => x.ProductMasters)
                                         .ToListAsync();
                                         
            return category;
        }
    }
}
