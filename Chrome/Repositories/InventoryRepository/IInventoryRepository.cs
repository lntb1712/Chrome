using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.InventoryRepository
{
    public interface IInventoryRepository:IRepositoryBase<Inventory>
    {
        Task<Inventory> GetInventoryWithCode(string locationCode, string productCode, string lotNo);
        IQueryable<Inventory> GetInventories(string[] warehouseCodes);
        IQueryable<Inventory> GetInventoriesByCategoryIds(string[] warehouseCodes, string[] categoryIds);
        IQueryable<Inventory> SearchProductInventories(string[] warehouseCodes, string textToSearch);
        IQueryable<Inventory> GetInventoryByProductCodeAsync(string productCode, string warehouseCode);
        IQueryable<Inventory> GetInventoryByWarehouseCodeAsync(string warehouseCode);
        IQueryable<Inventory> GetInventoryUsedPercent(string[]warehouseCodes);

    }
}
