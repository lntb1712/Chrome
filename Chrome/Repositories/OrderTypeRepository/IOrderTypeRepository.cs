using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.OrderTypeRepository
{
    public interface IOrderTypeRepository:IRepositoryBase<OrderType>
    {
        Task<List<OrderType>> GetAllOrderType();
        Task<List<OrderType>> GetOrderTypeByCode(string prefix);
    }
}
