using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Chrome.Repositories.StockInDetailRepository
{
    public class StockInDetailRepository:RepositoryBase<StockInDetail>, IStockInDetailRepository
    {
        private readonly ChromeContext _context;
        public StockInDetailRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<StockInDetail> GetAllStockInDetails(string stockInCode)
        {
            var lstStockInDetails = _context.StockInDetails
                                                  .Include(x => x.ProductCodeNavigation)
                                                  .Include(x=>x.StockInCodeNavigation)
                                                  .Where(x => x.StockInCode == stockInCode);
            return lstStockInDetails;
        }

        public async Task<StockInDetail> GetStockInDetailWithCode(string stockInCode, string productCode)
        {
            var stockInDetails = await _context.StockInDetails
                                         .Include(x => x.ProductCodeNavigation)
                                         .Include(x=>x.StockInCodeNavigation)
                                         .FirstOrDefaultAsync(x => x.StockInCode == stockInCode && x.ProductCode == productCode);
            return stockInDetails!;
        }
    }
}
