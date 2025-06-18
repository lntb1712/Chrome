using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.StockOutRepository
{
    public class StockOutRepository : RepositoryBase<StockOut>, IStockOutRepository
    {
        private readonly ChromeContext _context;

        public StockOutRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<StockOut> GetAllStockOutAsync(string[] warehouseCodes)
        {
            var lstStockOut = _context.StockOuts
                                     .Include(x => x.OrderTypeCodeNavigation)
                                     .Include(x => x.WarehouseCodeNavigation)
                                     .Include(x => x.CustomerCodeNavigation)
                                     .Include(x => x.ResponsibleNavigation)
                                     .Include(x => x.StockOutDetails)
                                     .Include(x => x.Status)
                                     .Where(x => warehouseCodes.Contains(x.WarehouseCode));

            return lstStockOut;
        }

        public IQueryable<StockOut> GetAllStockOutWithStatus(string[] warehouseCodes, int statusId)
        {
            var lstStockOut = _context.StockOuts
                                     .Include(x => x.OrderTypeCodeNavigation)
                                     .Include(x => x.WarehouseCodeNavigation)
                                     .Include(x => x.CustomerCodeNavigation)
                                     .Include(x => x.ResponsibleNavigation)
                                     .Include(x => x.StockOutDetails)
                                     .Include(x => x.Status)
                                     .Where(x => warehouseCodes.Contains(x.WarehouseCode) && x.StatusId == statusId);

            return lstStockOut;
        }

        public async Task<StockOut> GetStockOutWithCode(string stockOutCode)
        {
            var stockOut = await _context.StockOuts
                                        .Include(x => x.OrderTypeCodeNavigation)
                                        .Include(x => x.WarehouseCodeNavigation)
                                        .Include(x => x.CustomerCodeNavigation)
                                        .Include(x => x.ResponsibleNavigation)
                                        .Include(x => x.StockOutDetails)
                                        .Include(x => x.Status)
                                        .FirstOrDefaultAsync(x => x.StockOutCode == stockOutCode);

            return stockOut!;
        }

        public IQueryable<StockOut> SearchStockOutAsync(string[] warehouseCodes, string textToSearch)
        {
            var lstStockOut = _context.StockOuts
                                     .Include(x => x.OrderTypeCodeNavigation)
                                     .Include(x => x.WarehouseCodeNavigation)
                                     .Include(x => x.CustomerCodeNavigation)
                                     .Include(x => x.ResponsibleNavigation)
                                     .Include(x => x.StockOutDetails)
                                     .Include(x => x.Status)
                                     .Where(x => warehouseCodes.Contains(x.WarehouseCode)
                                     && (x.StockOutCode.Contains(textToSearch)
                                     || x.WarehouseCode!.Contains(textToSearch)
                                     || x.WarehouseCodeNavigation!.WarehouseName!.Contains(textToSearch)
                                     || x.OrderTypeCode!.Contains(textToSearch)
                                     || x.OrderTypeCodeNavigation!.OrderTypeName!.Contains(textToSearch)
                                     || x.CustomerCode!.Contains(textToSearch)
                                     || x.CustomerCodeNavigation!.CustomerName!.Contains(textToSearch)
                                     || x.Responsible!.Contains(textToSearch)
                                     || x.ResponsibleNavigation!.FullName!.Contains(textToSearch)));

            return lstStockOut;
        }
    }
}
