using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.TransferRepository
{
    public class TransferRepository : RepositoryBase<Transfer>, ITransferRepository
    {
        private readonly ChromeContext _context;

        public TransferRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Transfer> GetAllTransfersAsync(string[] warehouseCodes)
        {
            var lstTransfer = _context.Transfers
                .Include(t => t.OrderTypeCodeNavigation)
                .Include(t => t.FromWarehouseCodeNavigation)
                .Include(t => t.ToWarehouseCodeNavigation)
                .Include(t => t.FromResponsibleNavigation)
                .Include(t => t.ToResponsibleNavigation)
                .Include(t => t.Status)
                .Where(t => warehouseCodes.Contains(t.FromWarehouseCode) || warehouseCodes.Contains(t.ToWarehouseCode));

            return lstTransfer;
        }

        public IQueryable<Transfer> GetAllTransfersWithStatus(string[] warehouseCodes, int statusId)
        {
            var lstTransfer = _context.Transfers
                .Include(t => t.OrderTypeCodeNavigation)
                .Include(t => t.FromWarehouseCodeNavigation)
                .Include(t => t.ToWarehouseCodeNavigation)
                .Include(t => t.FromResponsibleNavigation)
                .Include(t => t.ToResponsibleNavigation)
                .Include(t => t.Status)
                .Where(t => (warehouseCodes.Contains(t.FromWarehouseCode) || warehouseCodes.Contains(t.ToWarehouseCode)) && t.StatusId == statusId);

            return lstTransfer;
        }

        public async Task<Transfer> GetTransferWithTransferCode(string transferCode)
        {
            var transfer = await _context.Transfers
                .Include(t => t.OrderTypeCodeNavigation)
                .Include(t => t.FromWarehouseCodeNavigation)
                .Include(t => t.ToWarehouseCodeNavigation)
                .Include(t => t.FromResponsibleNavigation)
                .Include(t => t.ToResponsibleNavigation)
                .Include(t => t.Status)
                .FirstOrDefaultAsync(t => t.TransferCode == transferCode);

            return transfer!;
        }

        public IQueryable<Transfer> SearchTransfersAsync(string[] warehouseCodes, string textToSearch)
        {
            var lstTransfer = _context.Transfers
                .Include(t => t.OrderTypeCodeNavigation)
                .Include(t => t.FromWarehouseCodeNavigation)
                .Include(t => t.ToWarehouseCodeNavigation)
                .Include(t => t.FromResponsibleNavigation)
                .Include(t => t.ToResponsibleNavigation)
                .Include(t => t.Status)
                .Where(t => (warehouseCodes.Contains(t.FromWarehouseCode) || warehouseCodes.Contains(t.ToWarehouseCode))
                    && (t.TransferCode.Contains(textToSearch)
                        || t.FromWarehouseCode!.Contains(textToSearch)
                        || t.FromWarehouseCodeNavigation!.WarehouseName!.Contains(textToSearch)
                        || t.ToWarehouseCode!.Contains(textToSearch)
                        || t.ToWarehouseCodeNavigation!.WarehouseName!.Contains(textToSearch)
                        || t.OrderTypeCode!.Contains(textToSearch)
                        || t.OrderTypeCodeNavigation!.OrderTypeName!.Contains(textToSearch)
                        || t.FromResponsible!.Contains(textToSearch)
                        || t.FromResponsibleNavigation!.FullName!.Contains(textToSearch)
                        || t.ToResponsible!.Contains(textToSearch)
                        || t.ToResponsibleNavigation!.FullName!.Contains(textToSearch)
                        || t.TransferDescription!.Contains(textToSearch)));

            return lstTransfer;
        }
    }
}
