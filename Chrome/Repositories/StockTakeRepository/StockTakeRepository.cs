using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.StockTakeRepository
{
    public class StockTakeRepository : RepositoryBase<Stocktake>, IStockTakeRepository
    {
        private readonly ChromeContext _context;

        public StockTakeRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Stocktake> GetAllStockTakesAsync(string[] warehouseCodes)
        {
            return _context.Stocktakes
                .Include(x => x.WarehouseCodeNavigation)
                .Include(x => x.ResponsibleNavigation)
                .Include(x => x.Status)
                .Where(st => warehouseCodes.Contains(st.WarehouseCode))
                .AsQueryable();
        }

        public IQueryable<Stocktake> GetStockTakeByStockTakeCodeAsync(string StockTakeCode)
        {
            return _context.Stocktakes
                .Include(x => x.WarehouseCodeNavigation)
                .Include(x => x.ResponsibleNavigation)
                .Include(x => x.Status)
                .Where(st => st.StocktakeCode == StockTakeCode)
                .AsQueryable();
        }

        public IQueryable<Stocktake> SearchStockTakesAsync(string[] warehouseCodes,  string textToSearch)
        {
            var query = _context.Stocktakes
                .Include(x => x.WarehouseCodeNavigation)
                .Include(x => x.ResponsibleNavigation)
                .Include(x => x.Status)
                .Where(st => warehouseCodes.Contains(st.WarehouseCode));

            if (!string.IsNullOrEmpty(textToSearch))
            {
                textToSearch = textToSearch.ToLower();
                query = query.Where(st =>
                    st.StocktakeCode.ToLower().Contains(textToSearch) ||
                    (st.WarehouseCodeNavigation != null && st.WarehouseCodeNavigation.WarehouseName != null && st.WarehouseCodeNavigation.WarehouseName.ToLower().Contains(textToSearch)) ||
                    (st.ResponsibleNavigation != null && st.ResponsibleNavigation.FullName != null && st.ResponsibleNavigation.FullName.ToLower().Contains(textToSearch)));
            }

            return query.AsQueryable();
        }
    }
}
