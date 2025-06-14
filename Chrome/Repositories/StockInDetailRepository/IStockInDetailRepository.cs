using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.StockInDetailRepository
{
    public interface IStockInDetailRepository:IRepositoryBase<StockInDetail>
    {
        Task<StockInDetail> GetStockInDetailWithCode(string stockInCode, string productCode);
        IQueryable<StockInDetail> GetAllStockInDetails(string stockInCode);
        
    }
}
