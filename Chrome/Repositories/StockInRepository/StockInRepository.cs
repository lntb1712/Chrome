using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Chrome.Repositories.StockInRepository
{
    public class StockInRepository:RepositoryBase<StockIn>,IStockInRepository
    {
        private readonly ChromeContext _context;
        public StockInRepository(ChromeContext context):base(context)
        {
            _context = context;
        }

        public  IQueryable<StockIn> GetAllStockInAsync(string[] warehouseCodes)
        {
            var lstStockIn = _context.StockIns
                                           .Include(x => x.OrderTypeCodeNavigation)
                                           .Include(x => x.WarehouseCodeNavigation)
                                           .Include(x => x.SupplierCodeNavigation)
                                           .Include(x => x.ResponsibleNavigation)
                                           .Include(x=>x.StockInDetails)
                                           .Include(x => x.Status)
                                           .Where(x => warehouseCodes.Contains(x.WarehouseCode));
                                          
            return lstStockIn;
        }

        public  IQueryable<StockIn> GetAllStockInWithStatus(string[] warehouseCodes, int statusId)
        {
            var lstStockIn = _context.StockIns
                                           .Include(x => x.OrderTypeCodeNavigation)
                                           .Include(x => x.WarehouseCodeNavigation)
                                           .Include(x => x.SupplierCodeNavigation)
                                           .Include(x => x.ResponsibleNavigation)
                                           .Include(x => x.StockInDetails)
                                           .Include(x => x.Status)
                                           .Where(x => warehouseCodes.Contains(x.WarehouseCode) && x.StatusId == statusId);
                                           
            return lstStockIn;

        }

        public async Task<StockIn> GetStockInWithCode(string stockInCode)
        {
            var stockIn = await _context.StockIns
                                        .Include(x => x.OrderTypeCodeNavigation)
                                        .Include(x => x.WarehouseCodeNavigation)
                                        .Include(x => x.SupplierCodeNavigation)
                                        .Include(x => x.ResponsibleNavigation)
                                        .Include(x => x.StockInDetails)
                                        .Include(x => x.Status)
                                        .FirstOrDefaultAsync(x => x.StockInCode == stockInCode);
            return stockIn!;

        }

        public IQueryable<StockIn> SearchStockInAsync(string[] warehouseCodes, string textToSearch)
        {
            var lstStockIn = _context.StockIns
                                     .Include(x => x.OrderTypeCodeNavigation)
                                     .Include(x => x.WarehouseCodeNavigation)
                                     .Include(x => x.SupplierCodeNavigation)
                                     .Include(x => x.ResponsibleNavigation)
                                     .Include(x => x.StockInDetails)
                                     .Include(x => x.Status)
                                     .Where(x => warehouseCodes.Contains(x.WarehouseCode)
                                     && (x.StockInCode.Contains(textToSearch)
                                     || x.WarehouseCode!.Contains(textToSearch)
                                     || x.WarehouseCodeNavigation!.WarehouseName!.Contains(textToSearch)
                                     || x.OrderTypeCode!.Contains(textToSearch)
                                     || x.OrderTypeCodeNavigation!.OrderTypeName!.Contains(textToSearch)
                                     || x.SupplierCode!.Contains(textToSearch)
                                     || x.SupplierCodeNavigation!.SupplierName!.Contains(textToSearch)
                                     || x.Responsible!.Contains(textToSearch)
                                     || x.ResponsibleNavigation!.FullName!.Contains(textToSearch)));
            return lstStockIn;
        }
    }
}
