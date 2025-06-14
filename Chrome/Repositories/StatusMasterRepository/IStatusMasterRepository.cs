using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.StatusMasterRepository
{
    public interface IStatusMasterRepository:IRepositoryBase<StatusMaster>
    {
        Task<List<StatusMaster>> GetAllStatuses();
    }
}
