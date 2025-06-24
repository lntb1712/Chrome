using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.StockTakeDetailRepository
{
    public class StocktakeDetailRepository : RepositoryBase<StocktakeDetail>, IStockTakeDetailRepository
    {
        private readonly ChromeContext _context;

        public StocktakeDetailRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<StocktakeDetail> GetAllStockTakeDetailsAsync(string[] warehouseCodes)
        {
            return _context.StocktakeDetails
                .Include(x => x.StocktakeCodeNavigation)
                .Include(x => x.ProductCodeNavigation)
                .Include(x => x.LocationCodeNavigation)
                .Where(sd => warehouseCodes.Contains(sd.StocktakeCodeNavigation.WarehouseCode))
                .AsQueryable();
        }

        public IQueryable<StocktakeDetail> GetStockTakeDetailsByStockTakeCodeAsync(string stocktakeCode)
        {
            return _context.StocktakeDetails
                .Include(x => x.StocktakeCodeNavigation)
                .Include(x => x.ProductCodeNavigation)
                .Include(x => x.LocationCodeNavigation)
                .Where(sd => sd.StocktakeCode == stocktakeCode)
                .AsQueryable();
        }

        public IQueryable<StocktakeDetail> SearchStockTakeDetailsAsync(string[] warehouseCodes, string stocktakeCode, string textToSearch)
        {
            var query = _context.StocktakeDetails
                .Include(x => x.StocktakeCodeNavigation)
                .Include(x => x.ProductCodeNavigation)
                .Include(x => x.LocationCodeNavigation)
                .Where(sd => warehouseCodes.Contains(sd.StocktakeCodeNavigation.WarehouseCode));

            if (!string.IsNullOrEmpty(stocktakeCode))
            {
                query = query.Where(sd => sd.StocktakeCode == stocktakeCode);
            }

            if (!string.IsNullOrEmpty(textToSearch))
            {
                textToSearch = textToSearch.ToLower();
                query = query.Where(sd =>
                    sd.StocktakeCode.ToLower().Contains(textToSearch) ||
                    sd.ProductCode.ToLower().Contains(textToSearch) ||
                    (sd.ProductCodeNavigation != null && sd.ProductCodeNavigation.ProductName != null && sd.ProductCodeNavigation.ProductName.ToLower().Contains(textToSearch)) ||
                    sd.Lotno.ToLower().Contains(textToSearch) ||
                    sd.LocationCode.ToLower().Contains(textToSearch));
            }

            return query.AsQueryable();
        }
    }
}
