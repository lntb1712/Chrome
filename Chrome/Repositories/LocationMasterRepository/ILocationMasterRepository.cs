using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.LocationMasterRepository
{
    public interface ILocationMasterRepository:IRepositoryBase<LocationMaster>
    {
        Task<List<LocationMaster>> GetAllLocationMaster(string warehouseCode,int page, int pageSize);
        Task<int> GetTotalLocationMasterCount(string warehouseCode);
        Task<LocationMaster> GetLocationMasterWithCode(string warehouseCode,string locationCode);
       
    }
}
