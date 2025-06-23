using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.MovementRepository
{
    public interface IMovementRepository:IRepositoryBase<Movement>
    {
        IQueryable<Movement> GetAllMovementAsync(string[] warehouseCodes);
        IQueryable<Movement> GetAllMovementWithStatus(string[] warehouseCodes, int StatusId);
        IQueryable<Movement> SearchMovementAsync (string[] warehouseCodes,string textToSearch);
        Task<Movement> GetMovementWithMovementCode(string movementCode);
    }
}
