using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.BOMMasterRepository
{
    public class BOMMasterRepository : RepositoryBase<Bommaster>, IBOMMasterRepository
    {
        private readonly ChromeContext _context;
        public BOMMasterRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Bommaster>> GetAllBOMMaster(int page, int pageSize)
        {
            var lstBOM = await _context.Bommasters
                                         .Include(x => x.ProductCodeNavigation)
                                         .OrderBy(x => x.Bomcode)
                                         .GroupBy(x => x.Bomcode)
                                         .Select(g => g.First())
                                         .Skip((page - 1) * pageSize)
                                         .Take(pageSize)
                                         .ToListAsync();
            return lstBOM;

        }

        public async Task<Bommaster> GetBOMMasterByCode(string bomCode, string bomVersion)
        {
            var bomMaster = await _context.Bommasters
                                    .Include(x => x.ProductCodeNavigation)
                                    .FirstOrDefaultAsync(x => x.Bomcode == bomCode && x.Bomversion == bomVersion);
            return bomMaster!;
        }

        public async Task<List<Bommaster>> GetListVersionByBomCode(string bomCode)
        {
            var bomVersions = await _context.Bommasters
                                      .Where(x => x.Bomcode == bomCode)
                                      .OrderBy(x => x.Bomversion)
                                      .ToListAsync();
            return bomVersions;
        }

        public async Task<int> GetTotalBOMMasterCount()
        {
            var totalCount = await _context.Bommasters.CountAsync();
            return totalCount;
        }

        public async Task<int> GetTotalSearchBOMMasterCount(string textToSearch)
        {
            var totalCount = await _context.Bommasters
                .Where(x => x.Bomcode.Contains(textToSearch)
                || x.ProductCodeNavigation!.ProductName!.Contains(textToSearch))
                .CountAsync();
            return totalCount;
        }

        public async Task<List<Bommaster>> SearchBOMMaster(string textToSearch, int page, int pageSize)
        {
            return await _context.Bommasters
                .Include(x => x.ProductCodeNavigation)
                .Where(x => x.Bomcode.Contains(textToSearch)
                || x.ProductCodeNavigation!.ProductName!.Contains(textToSearch))
                .OrderBy(x => x.Bomcode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}