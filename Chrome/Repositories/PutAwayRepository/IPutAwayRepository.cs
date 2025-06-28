using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.PutawayRepository
{
    public interface IPutAwayRepository : IRepositoryBase<PutAway>
    {
        IQueryable<PutAway> GetAllPutAwayAsync(string[] warehouseCodes);
        IQueryable<PutAway> GetAllPutAwayWithStatus(string[] warehouseCodes, int statusId);
        IQueryable<PutAway> SearchPutAwayAsync(string[] warehouseCodes, string textToSearch);
        Task<PutAway> GetPutAwayCodeAsync(string putawayNo);
        Task<PutAway> GetPutAwayContainsCodeAsync(string orderCode);
        Task<PutAway> GetPutAwayContainsMovement(string movementCode);
    }
}