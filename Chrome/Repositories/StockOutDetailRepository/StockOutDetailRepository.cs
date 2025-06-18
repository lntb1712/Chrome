using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.StockOutDetailRepository
{
    public class StockOutDetailRepository : RepositoryBase<StockOutDetail>, IStockOutDetailRepository
    {
        private readonly ChromeContext _context;

        public StockOutDetailRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<StockOutDetail> GetAllStockOutDetails(string stockOutCode)
        {
            var lstStockOutDetails = _context.StockOutDetails
                                            .Include(x => x.ProductCodeNavigation)
                                            .Include(x => x.StockOutCodeNavigation)
                                            .Where(x => x.StockOutCode == stockOutCode);
            return lstStockOutDetails;
        }

        public async Task<StockOutDetail> GetStockOutDetailWithCode(string stockOutCode, string productCode)
        {
            var stockOutDetails = await _context.StockOutDetails
                                               .Include(x => x.ProductCodeNavigation)
                                               .Include(x => x.StockOutCodeNavigation)
                                               .FirstOrDefaultAsync(x => x.StockOutCode == stockOutCode && x.ProductCode == productCode);
            return stockOutDetails!;
        }
    }
}
