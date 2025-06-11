using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.ProductMasterRepository
{
    public interface IProductMasterRepository:IRepositoryBase<ProductMaster>    
    {
        Task<List<ProductMaster>> GetAllProduct(int page, int pageSize);
        Task<int> GetTotalProductCount();
        Task<ProductMaster> GetProductMasterWithProductCode(string productCode);
        Task<List<ProductMaster>> SearchProduct(string textToSearch, int page, int pageSize);
        Task<int> GetTotalSearchCount(string textToSearch);
        Task<List<ProductMaster>> GetAllProductWithCategoryID(string categoryId,int page, int pageSize);
        Task<int> GetTotalProductWithCategoryIDCount(string categoryId);
        Task<List<ProductMaster>> GetProductMasterWithCategoryID(string[] categoryIds);
    }
}
