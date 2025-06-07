using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.ProductCustomerRepository
{
    public interface IProductCustomerRepository:IRepositoryBase<CustomerProduct>
    {
        Task<List<CustomerProduct>> GetAllCustomerProducts(string productCode,int page, int pageSize);
        Task<int> GetTotalCustomerProductCount(string productCode);
        Task<CustomerProduct> GetAllCustomerProductsByCode(string productCode, string customerCode);
    }
}
