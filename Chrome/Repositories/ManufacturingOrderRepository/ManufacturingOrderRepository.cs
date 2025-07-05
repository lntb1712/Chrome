using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.ManufacturingOrderRepository
{
    public class ManufacturingOrderRepository :RepositoryBase<ManufacturingOrder>, IManufacturingOrderRepository
    {
        private readonly ChromeContext _context;
        public ManufacturingOrderRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<ManufacturingOrder> GetAllManufacturingOrder(string[] warehouseCodes)
        {
            var lstManufact = _context.ManufacturingOrders
                                      .Include(x=>x.ManufacturingOrderDetails)
                                      .Include(x => x.OrderTypeCodeNavigation)
                                      .Include(x => x.WarehouseCodeNavigation)
                                      .Include(x => x.ProductCodeNavigation)
                                      .Include(x => x.ResponsibleNavigation)
                                      .Include(x => x.Status)
                                      .Include(x => x.Bommaster)
                                      .Where(x => warehouseCodes.Contains(x.WarehouseCode));
            return lstManufact;

        }

        public IQueryable<ManufacturingOrder> GetAllManufacturingOrderWithStatus(string[] warehouseCodes, int statusId)
        {
            var lstManufact = _context.ManufacturingOrders
                                      .Include(x => x.OrderTypeCodeNavigation)
                                      .Include(x => x.WarehouseCodeNavigation)
                                      .Include(x => x.ProductCodeNavigation)
                                      .Include(x => x.ResponsibleNavigation)
                                      .Include(x => x.Status)
                                      .Include(x => x.Bommaster)
                                      .Where(x => warehouseCodes.Contains(x.WarehouseCode) && x.StatusId == statusId);
            return lstManufact;
        }

        public async Task<ManufacturingOrder> GetManufacturingWithCode(string manufacturingCode)
        {
            var manufacturingOrder = await _context.ManufacturingOrders
                                                   .Include(x => x.OrderTypeCodeNavigation)
                                      .Include(x => x.WarehouseCodeNavigation)
                                      .Include(x => x.ProductCodeNavigation)
                                      .Include(x => x.ResponsibleNavigation)
                                      .Include(x => x.Status)
                                      .Include(x => x.Bommaster)
                                      .FirstOrDefaultAsync(x=>x.ManufacturingOrderCode == manufacturingCode);
            return manufacturingOrder!;

        }

        public IQueryable<ManufacturingOrder> SearchManufacturingAsync(string[] warehouseCodes, string textToSearch)
        {
            var lstManufact = _context.ManufacturingOrders
                                      .Include(x => x.OrderTypeCodeNavigation)
                                      .Include(x => x.WarehouseCodeNavigation)
                                      .Include(x => x.ProductCodeNavigation)
                                      .Include(x => x.ResponsibleNavigation)
                                      .Include(x => x.Status)
                                      .Include(x => x.Bommaster)
                                      .Where(x => warehouseCodes.Contains(x.WarehouseCode) 
                                      && (x.ManufacturingOrderCode.Contains(textToSearch)
                                      || x.OrderTypeCode!.Contains(textToSearch)
                                      || x.OrderTypeCodeNavigation!.OrderTypeName!.Contains(textToSearch)
                                      || x.ProductCode!.Contains(textToSearch)
                                      || x.ProductCodeNavigation!.ProductName!.Contains(textToSearch)
                                      || x.Bomcode!.Contains(textToSearch)
                                      || x.BomVersion!.Contains(textToSearch)
                                      || x.Responsible!.Contains(textToSearch)
                                      || x.ResponsibleNavigation!.FullName!.Contains(textToSearch)
                                      || x.Lotno!.Contains(textToSearch)
                                      || x.WarehouseCode!.Contains(textToSearch)
                                      || x.WarehouseCodeNavigation!.WarehouseName!.Contains(textToSearch)));
            return lstManufact;
        }
    }
}
