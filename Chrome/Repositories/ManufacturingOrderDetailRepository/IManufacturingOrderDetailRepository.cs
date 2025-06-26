using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.ManufacturingOrderDetailRepository
{
    public interface IManufacturingOrderDetailRepository : IRepositoryBase<ManufacturingOrderDetail>
    {
        IQueryable<ManufacturingOrderDetail> GetManufacturingOrderDetail(string manufacturingOrderCode);
        Task<ManufacturingOrderDetail> GetManufacturingOrderDetailWithCode(string manufacturingCode,string componentCode);
    }
}
