using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;


namespace Chrome.Repositories.GroupManagementRepository
{
    public class GroupManagementRepository : RepositoryBase<GroupManagement>, IGroupManagementRepository
    {
        private readonly ChromeContext _context;

        public GroupManagementRepository(ChromeContext context) : base(context)
        {

            _context = context;
        }

        public async Task<List<GroupManagement>> GetAllGroup(int page, int pageSize)
        {
            var lstGroup = await _context.GroupManagements
                                     .Include(x => x.GroupFunctions)
                                     .OrderBy(x => x.GroupId)
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();
            return lstGroup; 
        }

        public async Task<GroupManagement> GetGroupManagementWithGroupID(string GroupID)
        {
            //throw new NotImplementedException();
            //var group = await GetByIDAsync(GroupID);
            var group = await _context.GroupManagements
                        .Include(x => x.GroupFunctions)
                        .FirstOrDefaultAsync(row => row.GroupId == GroupID);
            return group!;
        }

        public async Task<int> GetTotalGroupCount()
        {
            return await _context.GroupManagements.Include(x=>x.GroupFunctions).CountAsync();
        }

        public async Task<int> GetTotalSearchCount(string textToSearch)
        {
            var group = await _context.GroupManagements
                                   .Include(x => x.GroupFunctions)
                                   .Where(x => x.GroupId.Contains(textToSearch) || x.GroupName!.Contains(textToSearch) || x.GroupDescription!.Contains(textToSearch))
                                   .CountAsync();
            return group;       
        }

        public async Task<Dictionary<string, int>> GetTotalUserInGroup()
        {
            var group =  await _context.GroupManagements
                                .Include(x => x.GroupFunctions)
                                .Include(x => x.AccountManagements)
                                .Select(x => new
                                {
                                    GroupName = x.GroupName,
                                    TotalUser = x.AccountManagements.Where(t => t.GroupId == x.GroupId).Count()
                                }).ToDictionaryAsync(x=>x.GroupName!,x=>x.TotalUser!);
            return group;
        }

        public async Task<List<GroupManagement>> SearchGroup(string textToSearch, int page, int pageSize)
        {
            var group = await _context.GroupManagements
                                   .Include(x => x.GroupFunctions)
                                   .Where(x => x.GroupId.Contains(textToSearch) || x.GroupName!.Contains(textToSearch) || x.GroupDescription!.Contains(textToSearch))
                                   .OrderBy(x => x.GroupId)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            return group;
        }
    }

}
