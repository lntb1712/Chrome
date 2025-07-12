using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.PurchaseOrderRepository
{
    public interface IPurchaseOrderRepository:IRepositoryBase<PurchaseOrder>
    {
        IQueryable<PurchaseOrder> GetAllPurchaseOrdersAsync(string[] warehouseCodes);
        IQueryable<PurchaseOrder> GetAllPurchaseOrderWithStatus(string[] warehouseCodes, int statusId);
        IQueryable<PurchaseOrder> SearchPurchaseOrderAsync(string[] warehouseCodes, string textToSearch);
        Task<PurchaseOrder> GetPurchaseOrderWithCode(string purchaseOrderCode);

    }
}
