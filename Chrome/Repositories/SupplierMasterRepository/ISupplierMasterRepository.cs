using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.SupplierMasterRepository
{
    public interface ISupplierMasterRepository:IRepositoryBase<SupplierMaster>
    {
        Task<List<SupplierMaster>> GetAllSupplier(int page, int pageSize);
        Task<int>GetTotalSupplierCount();   
        Task<List<SupplierMaster>> SearchSupplier(string textToSearch,int page, int pageSize);
        Task<int>GetTotalSearchCount(string textToSearch);
        Task<SupplierMaster> GetSupplierMasterWithSupplierCode(string supplierCode);

    }
}
