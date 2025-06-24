using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.TransferDetailRepository
{
    public class TransferDetailRepository : RepositoryBase<TransferDetail>, ITransferDetailRepository
    {
        private readonly ChromeContext _context;

        public TransferDetailRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<TransferDetail> GetAllTransferDetailsAsync(string[] warehouseCodes)
        {
            return _context.TransferDetails
                .Include(x => x.TransferCodeNavigation)
                .Include(x => x.ProductCodeNavigation)
                .Where(td => warehouseCodes.Contains(td.TransferCodeNavigation.FromWarehouseCode) || warehouseCodes.Contains(td.TransferCodeNavigation.ToWarehouseCode))
                .AsQueryable();
        }

        public IQueryable<TransferDetail> GetTransferDetailsByTransferCodeAsync(string transferCode)
        {
            return _context.TransferDetails
                .Include(x => x.TransferCodeNavigation)
                .Include(x => x.ProductCodeNavigation)
                .Where(td => td.TransferCode == transferCode)
                .AsQueryable();
        }

        public IQueryable<TransferDetail> SearchTransferDetailsAsync(string[] warehouseCodes, string transferCode, string textToSearch)
        {
            var query = _context.TransferDetails
                .Include(x => x.TransferCodeNavigation)
                .Include(x => x.ProductCodeNavigation)
                .Where(td => warehouseCodes.Contains(td.TransferCodeNavigation.FromWarehouseCode) || warehouseCodes.Contains(td.TransferCodeNavigation.ToWarehouseCode));

            if (!string.IsNullOrEmpty(transferCode))
            {
                query = query.Where(td => td.TransferCode == transferCode);
            }

            if (!string.IsNullOrEmpty(textToSearch))
            {
                textToSearch = textToSearch.ToLower();
                query = query.Where(td =>
                    td.TransferCode.ToLower().Contains(textToSearch) ||
                    (td.ProductCode != null && td.ProductCode.ToLower().Contains(textToSearch)) ||
                    (td.ProductCodeNavigation != null && td.ProductCodeNavigation.ProductName != null &&
                     td.ProductCodeNavigation.ProductName.ToLower().Contains(textToSearch)));
            }

            return query.AsQueryable();
        }
    }
}
