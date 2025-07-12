using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.PurchaseOrderRepository
{
    public class PurchaseOrderRepository:RepositoryBase<PurchaseOrder>,IPurchaseOrderRepository
    {
        private readonly ChromeContext _context;
        public PurchaseOrderRepository(ChromeContext context):base(context)
        {
            _context = context;
        }

        public IQueryable<PurchaseOrder> GetAllPurchaseOrdersAsync(string[] warehouseCodes)
        {
            var query = _context.PurchaseOrders
                .Include(po => po.WarehouseCodeNavigation)
                .Include(po => po.SupplierCodeNavigation)
                .Include(po => po.Status)
                .Where(po => warehouseCodes.Contains(po.WarehouseCode))
                .AsQueryable();
            return query;
        }

        public IQueryable<PurchaseOrder> GetAllPurchaseOrderWithStatus(string[] warehouseCodes, int statusId)
        {
            var query = _context.PurchaseOrders
                .Include(po => po.WarehouseCodeNavigation)
                .Include(po => po.SupplierCodeNavigation)
                .Include(po => po.Status)
                .Where(po => warehouseCodes.Contains(po.WarehouseCode) && po.StatusId == statusId)
                .AsQueryable();
            return query;
        }

        public async  Task<PurchaseOrder> GetPurchaseOrderWithCode(string purchaseOrderCode)
        {
            var query = await _context.PurchaseOrders
                .Include(po => po.WarehouseCodeNavigation)
                .Include(po => po.SupplierCodeNavigation)
                .Include(po => po.Status)
                .Include(po => po.PurchaseOrderDetails)
                .ThenInclude(pod => pod.ProductCodeNavigation)
                .Where(po => po.PurchaseOrderCode == purchaseOrderCode)
                .FirstOrDefaultAsync();
            return query!;
        }

        public IQueryable<PurchaseOrder> SearchPurchaseOrderAsync(string[] warehouseCodes, string textToSearch)
        {
            var query = _context.PurchaseOrders
                .Include(po => po.WarehouseCodeNavigation)
                .Include(po => po.SupplierCodeNavigation)
                .Include(po => po.Status)
                .Where(po => warehouseCodes.Contains(po.WarehouseCode) && 
                             (po.PurchaseOrderCode.Contains(textToSearch) || 
                              po.SupplierCodeNavigation!.SupplierName!.Contains(textToSearch)))
                .AsQueryable();
            return query!;
        }
    }
}
