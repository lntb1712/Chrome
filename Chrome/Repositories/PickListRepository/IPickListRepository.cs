using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.PickListRepository
{
    public interface IPickListRepository:IRepositoryBase<PickList>
    {
        IQueryable<PickList> GetAllPickListAsync(string[] warehouseCodes);
        IQueryable<PickList> GetAllPickListWithStatus(string[] warehouseCodes, int statusId);
        IQueryable<PickList> SearchPickListAsync(string[] warehouseCodes, string textToSearch);
        Task<PickList> GetPickListWithCode(string pickNo);
        Task<PickList> GetPickListContainCode(string movementCode);
    }
}
