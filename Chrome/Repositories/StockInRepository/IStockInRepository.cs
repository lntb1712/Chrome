using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.StockInRepository
{
    public interface IStockInRepository:IRepositoryBase<StockIn>
    {
        IQueryable<StockIn> GetAllStockInAsync(string[] warehouseCodes);
        IQueryable<StockIn> GetAllStockInWithStatus(string[] warehouseCodes, int statusId);
        IQueryable<StockIn> SearchStockInAsync(string[]warehouseCodes, string textToSearch);
        Task<StockIn> GetStockInWithCode(string stockInCode);
    }
}
