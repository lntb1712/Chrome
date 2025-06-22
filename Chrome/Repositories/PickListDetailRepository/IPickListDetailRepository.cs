using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.PickListDetailRepository
{
    public interface IPickListDetailRepository : IRepositoryBase<PickListDetail>
    {
        IQueryable<PickListDetail> GetAllPickListDetailsAsync(string[] warehouseCodes);
        IQueryable<PickListDetail> GetPickListDetailsByPickNoAsync(string pickNo);
        IQueryable<PickListDetail> SearchPickListDetailsAsync(string[] warehouseCodes,string pickNo, string textToSearch);
    }
}
