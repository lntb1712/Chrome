using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.CategoryRepository
{
    public interface ICategoryRepository:IRepositoryBase<Category>
    {
        Task<List<Category>> GetAllCategories();
        Task<List<Category>>GetTotalProductInCategory();

    }
}
