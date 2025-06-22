using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.PutAwayDetailRepository
{
    public interface IPutAwayDetailRepository:IRepositoryBase<PutAwayDetail>
    {
        IQueryable<PutAwayDetail> GetAllPutAwayDetailsAsync(string[] warehouseCodes);
        IQueryable<PutAwayDetail> GetPutAwayDetailsByPutawayNoAsync(string putawayNo);
        IQueryable<PutAwayDetail> SearchPutAwayDetailsAsync(string[] warehouseCodes, string putawayNo, string textToSearch);
    }
}
