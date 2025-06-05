using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.ProductSupplierRepository
{
    public interface IProductSupplierRepository:IRepositoryBase<ProductSupplier>
    {
        Task<List<ProductSupplier>>GetAllProductSupplierWithProductCode(string productCode,int page, int pageSize);
        Task<int> GetTotalProductSupplierProductCodeCount(string productCode);
        Task<ProductSupplier> GetProductSupplierWithProductCodeAndSupplierCode(string productCode,string supplierCode);
    }
}
