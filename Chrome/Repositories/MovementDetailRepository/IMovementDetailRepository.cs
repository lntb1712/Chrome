using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.MovementDetailRepository
{
    public interface IMovementDetailRepository : IRepositoryBase<MovementDetail>
    {
        IQueryable<MovementDetail> GetAllMovementDetailsAsync(string[] warehouseCodes);
        IQueryable<MovementDetail> GetMovementDetailsByMovementCodeAsync(string movementCode);
        IQueryable<MovementDetail> SearchMovementDetailsAsync(string[] warehouseCodes, string movementCode, string textToSearch);
    }
}