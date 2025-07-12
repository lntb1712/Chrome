using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.PurchaseOrderDetailRepository
{
    public class PurchaseOrderDetailRepository :RepositoryBase<PurchaseOrderDetail>, IPurchaseOrderDetailRepository
    {
        private readonly ChromeContext _context;
        public PurchaseOrderDetailRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<PurchaseOrderDetail> GetAllPurchaseOrderDetailsAsync(string purchaseOrderCode)
        {
            var query = _context.PurchaseOrderDetails
                .Include(pod => pod.ProductCodeNavigation)
                .Include(pod=> pod.PurchaseOrderCodeNavigation)
                .Where(pod => pod.PurchaseOrderCode == purchaseOrderCode)
                .AsQueryable();
            return query;
        }

        public async Task<PurchaseOrderDetail> GetPurchaseOrderDetailWithCode(string purchaseOrderCode, string productCode)
        {
            var purchaseOrderDetail = await _context.PurchaseOrderDetails
                .Include(pod => pod.ProductCodeNavigation)
                .Include(pod => pod.PurchaseOrderCodeNavigation)
                .Where(pod => pod.PurchaseOrderCode == purchaseOrderCode && pod.ProductCode == productCode)
                .FirstOrDefaultAsync();
            return purchaseOrderDetail!;
        }
    }
}
