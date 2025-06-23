using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Chrome.Repositories.MovementRepository
{
    public class MovementRepository : RepositoryBase<Movement>, IMovementRepository
    {
        private readonly ChromeContext _context;

        public MovementRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Movement> GetAllMovementAsync(string[] warehouseCodes)
        {
            var lstMovement = _context.Movements
                .Include(m => m.OrderTypeCodeNavigation)
                .Include(m => m.WarehouseCodeNavigation)
                .Include(m => m.ResponsibleNavigation)
                .Include(m => m.Status)
                .Where(m => warehouseCodes.Contains(m.WarehouseCode));

            return lstMovement;
        }

        public IQueryable<Movement> GetAllMovementWithStatus(string[] warehouseCodes, int statusId)
        {
            var lstMovement = _context.Movements
                .Include(m => m.OrderTypeCodeNavigation)
                .Include(m => m.WarehouseCodeNavigation)
                .Include(m => m.ResponsibleNavigation)
                .Include(m => m.Status)
                .Where(m => warehouseCodes.Contains(m.WarehouseCode) && m.StatusId == statusId);

            return lstMovement;
        }

        public async Task<Movement> GetMovementWithMovementCode(string movementCode)
        {
            var movement = await _context.Movements
                .Include(m => m.OrderTypeCodeNavigation)
                .Include(m => m.WarehouseCodeNavigation)
                .Include(m => m.ResponsibleNavigation)
                .Include(m => m.Status)
                .FirstOrDefaultAsync(m => m.MovementCode == movementCode);

            return movement!;
        }

        public IQueryable<Movement> SearchMovementAsync(string[] warehouseCodes, string textToSearch)
        {
            var lstMovement = _context.Movements
                .Include(m => m.OrderTypeCodeNavigation)
                .Include(m => m.WarehouseCodeNavigation)
                .Include(m => m.ResponsibleNavigation)
                .Include(m => m.Status)
                .Where(m => warehouseCodes.Contains(m.WarehouseCode)
                    && (m.MovementCode.Contains(textToSearch)
                        || m.WarehouseCode!.Contains(textToSearch)
                        || m.WarehouseCodeNavigation!.WarehouseName!.Contains(textToSearch)
                        || m.OrderTypeCode!.Contains(textToSearch)
                        || m.OrderTypeCodeNavigation!.OrderTypeName!.Contains(textToSearch)
                        || m.Responsible!.Contains(textToSearch)
                        || m.ResponsibleNavigation!.FullName!.Contains(textToSearch)
                        || m.FromLocation!.Contains(textToSearch)
                        || m.ToLocation!.Contains(textToSearch)
                        || m.MovementDescription!.Contains(textToSearch)));

            return lstMovement;
        }
    }
}