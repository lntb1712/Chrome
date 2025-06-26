using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Chrome.Repositories.ManufacturingOrderDetailRepository
{
    public class ManufacturingOrderDetailRepository:RepositoryBase<ManufacturingOrderDetail>,IManufacturingOrderDetailRepository
    {
        private readonly ChromeContext _context;
        public ManufacturingOrderDetailRepository(ChromeContext context):base(context)
        {
            _context = context;
        }

        public IQueryable<ManufacturingOrderDetail>GetManufacturingOrderDetail(string manufacturingOrderCode)
        {
            var detail = _context.ManufacturingOrderDetails
                                 .Include(x => x.ManufacturingOrderCodeNavigation)
                                 .Include(x => x.ComponentCodeNavigation)
                                 .AsQueryable()
                                 .Where(x=>x.ManufacturingOrderCode==manufacturingOrderCode);
            return detail;
        }

        public async Task<ManufacturingOrderDetail> GetManufacturingOrderDetailWithCode(string manufacturingCode, string componentCode)
        {
            var detail =await _context.ManufacturingOrderDetails
                                 .Include(x => x.ManufacturingOrderCodeNavigation)
                                 .Include(x => x.ComponentCodeNavigation)
                                 .FirstOrDefaultAsync(x => x.ManufacturingOrderCode == manufacturingCode && x.ComponentCode==componentCode);
            return detail!;
        }
    }
}
