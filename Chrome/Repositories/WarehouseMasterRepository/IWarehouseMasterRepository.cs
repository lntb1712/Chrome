using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.WarehouseMasterRepository
{
    public interface IWarehouseMasterRepository:IRepositoryBase<WarehouseMaster>
    {
        Task<List<WarehouseMaster>> GetWarehouseMasters(int page,int pageSize);
        Task<int> GetTotalWarehouse();
        Task<WarehouseMaster> GetWarehouseMasterWithCode(string warehouseCode); 
        Task<List<WarehouseMaster>> SearchWarehouse(string textToSearch, int page, int pageSize);
        Task<int> GetTotalSearchCount(string textToSearch);

    }
}
