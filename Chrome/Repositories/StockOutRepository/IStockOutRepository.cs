using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.StockOutRepository
{
    public interface IStockOutRepository: IRepositoryBase<StockOut>
    {
        IQueryable<StockOut> GetAllStockOutAsync(string[] warehouseCodes);
        IQueryable<StockOut> GetAllStockOutWithStatus(string[] warehouseCodes, int statusId);
        IQueryable<StockOut> SearchStockOutAsync(string[] warehouseCodes, string textToSearch);
        Task<StockOut> GetStockOutWithCode(string stockOutCode);
    }
}
