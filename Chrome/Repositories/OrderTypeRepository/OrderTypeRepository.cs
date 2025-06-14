using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.OrderTypeRepository
{
    public class OrderTypeRepository:RepositoryBase<OrderType>, IOrderTypeRepository
    {
        private readonly ChromeContext _context;
        public OrderTypeRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<OrderType>> GetAllOrderType()
        {
            return await _context.OrderTypes.ToListAsync();
        }

        public async Task<List<OrderType>> GetOrderTypeByCode(string prefix)
        {
            var orderType = await _context.OrderTypes
                                          .Where(x => x.OrderTypeCode.StartsWith(prefix))
                                          .ToListAsync();
            return orderType!;
        }
    }
}
