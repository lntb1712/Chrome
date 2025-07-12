using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.PurchaseOrderDetailRepository
{
    public interface IPurchaseOrderDetailRepository :IRepositoryBase<PurchaseOrderDetail>
    {
        Task<PurchaseOrderDetail> GetPurchaseOrderDetailWithCode(string purchaseOrderCode, string productCode);
        IQueryable<PurchaseOrderDetail> GetAllPurchaseOrderDetailsAsync(string purchaseOrderCode);
    }
}
