using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.StockTakeRepository
{
    public interface IStockTakeRepository : IRepositoryBase<Stocktake>
    {
        IQueryable<Stocktake> GetAllStockTakesAsync(string[] warehouseCodes);
        IQueryable<Stocktake> GetStockTakeByStockTakeCodeAsync(string StockTakeCode);
        IQueryable<Stocktake> SearchStockTakesAsync(string[] warehouseCodes, string StockTakeCode, string textToSearch);
    }
}
