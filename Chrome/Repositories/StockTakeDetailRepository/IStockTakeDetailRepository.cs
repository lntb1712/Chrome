using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.StockTakeDetailRepository
{
    public interface IStockTakeDetailRepository : IRepositoryBase<StocktakeDetail>
    {
        IQueryable<StocktakeDetail> GetAllStockTakeDetailsAsync(string[] warehouseCodes);
        IQueryable<StocktakeDetail> GetStockTakeDetailsByStockTakeCodeAsync(string StockTakeCode);
        IQueryable<StocktakeDetail> SearchStockTakeDetailsAsync(string[] warehouseCodes, string StockTakeCode, string textToSearch);
    }
}
