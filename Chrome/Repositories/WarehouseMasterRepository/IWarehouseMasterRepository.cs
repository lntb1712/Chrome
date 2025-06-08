using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.WarehouseMasterRepository
{
    public interface IWarehouseMasterRepository:IRepositoryBase<WarehouseMaster>
    {
        Task<List<WarehouseMaster>> GetWarehouseMasters(int page,int pageSize);
        Task<int> GetTotalWarehouse();
    }
}
