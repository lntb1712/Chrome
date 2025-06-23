using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.MovementDetailRepository
{
   

    public class MovementDetailRepository : RepositoryBase<MovementDetail>, IMovementDetailRepository
    {
        private readonly ChromeContext _context;

        public MovementDetailRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<MovementDetail> GetAllMovementDetailsAsync(string[] warehouseCodes)
        {
            return _context.MovementDetails
                .Include(x => x.MovementCodeNavigation)
                .Include(x => x.ProductCodeNavigation)
                .Where(md => warehouseCodes.Contains(md.MovementCodeNavigation.WarehouseCode))
                .AsQueryable();
        }

        public IQueryable<MovementDetail> GetMovementDetailsByMovementCodeAsync(string movementCode)
        {
            return _context.MovementDetails
                .Include(x => x.MovementCodeNavigation)
                .Include(x => x.ProductCodeNavigation)
                .Where(md => md.MovementCode == movementCode)
                .AsQueryable();
        }

        public IQueryable<MovementDetail> SearchMovementDetailsAsync(string[] warehouseCodes, string movementCode, string textToSearch)
        {
            var query = _context.MovementDetails
                .Include(x => x.MovementCodeNavigation)
                .Include(x => x.ProductCodeNavigation)
                .Where(md => warehouseCodes.Contains(md.MovementCodeNavigation.WarehouseCode));

            if (!string.IsNullOrEmpty(movementCode))
            {
                query = query.Where(md => md.MovementCode == movementCode);
            }

            if (!string.IsNullOrEmpty(textToSearch))
            {
                textToSearch = textToSearch.ToLower();
                query = query.Where(md =>
                    md.MovementCode.ToLower().Contains(textToSearch) ||
                    (md.ProductCode != null && md.ProductCode.ToLower().Contains(textToSearch)) ||
                    (md.ProductCodeNavigation != null && md.ProductCodeNavigation.ProductName != null &&
                     md.ProductCodeNavigation.ProductName.ToLower().Contains(textToSearch)));
            }

            return query.AsQueryable();
        }
    }
}