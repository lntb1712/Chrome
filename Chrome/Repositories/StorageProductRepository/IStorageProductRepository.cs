using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.StorageProductRepository
{
    public interface IStorageProductRepository:IRepositoryBase<StorageProduct>
    {
        Task<List<StorageProduct>> GetAllStorageProducts(int page, int pageSize);
        Task<int> GetTotalStorageProductCount();
        Task<StorageProduct> GetStorageProductWithCode(string storageProductCode);
        Task<List<StorageProduct>> SearchStorageProducts(string textToSearch, int page, int pageSize);
        Task<int> GetTotalSearchCount(string textToSearch);
    }
}
