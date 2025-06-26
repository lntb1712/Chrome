using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.ManufacturingOrderRepository
{
    public interface IManufacturingOrderRepository:IRepositoryBase<ManufacturingOrder>
    {
        IQueryable<ManufacturingOrder> GetAllManufacturingOrder(string[] warehouseCodes);
        IQueryable<ManufacturingOrder> GetAllManufacturingOrderWithStatus(string[] warehouseCodes, int statusId);
        IQueryable<ManufacturingOrder> SearchManufacturingAsync(string[] warehouseCodes,string textToSearch);
        Task<ManufacturingOrder> GetManufacturingWithCode(string manufacturingCode);
    }
}
