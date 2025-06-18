using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.StockOutDetailRepository
{
    public interface IStockOutDetailRepository : IRepositoryBase<StockOutDetail>
    {
        Task<StockOutDetail> GetStockOutDetailWithCode(string stockOutCode, string productCode);
        IQueryable<StockOutDetail> GetAllStockOutDetails(string stockOutCode);
    }
}
