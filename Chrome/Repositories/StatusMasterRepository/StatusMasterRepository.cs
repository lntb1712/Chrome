using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.StatusMasterRepository
{
    public class StatusMasterRepository : RepositoryBase<StatusMaster>, IStatusMasterRepository
    {
        private readonly ChromeContext _context;
        public StatusMasterRepository(ChromeContext context):base(context)
        {
            _context = context;
        }

        public async Task<List<StatusMaster>> GetAllStatuses()
        {
            return await _context.StatusMasters.ToListAsync();
        }
    }
}
