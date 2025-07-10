using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.ReplenishRepository
{
    public interface IReplenishRepository:IRepositoryBase<Replenish>
    {
        IQueryable<Replenish> GetAllReplenishAsync(string warehouseCode);
        Task<Replenish> GetReplenishByCode(string productCode, string warehouseCode);
        IQueryable<Replenish> SearchReplenishAsync(string warehouseCode, string textToSearch);
    }
}
