using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.StockInRepository
{

    public interface IStockInRepository:IRepositoryBase<StockIn>
    {
        IQueryable<StockIn> GetAllStockInAsync(string[] warehouseCodes);
        IQueryable<StockIn> GetAllStockInWithResponsible(string[] warehouseCodes, string responsible);
        IQueryable<StockIn> GetAllStockInWithStatus(string[] warehouseCodes, int statusId);
        IQueryable<StockIn> SearchStockInAsync(string[]warehouseCodes, string textToSearch);
        IQueryable<StockIn> SearchStockInWithResponsible(string[] warehouseCodes, string responsible, string textToSearch);
        Task<StockIn> GetStockInWithCode(string stockInCode);
    }
}
