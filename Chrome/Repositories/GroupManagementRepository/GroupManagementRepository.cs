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

        public async Task<List<GroupManagement>> GetAllGroup()
        {
            var lstGroup = await _context.GroupManagements
                                    .Include(x=>x.GroupFunctions)
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

        public async Task<List<GroupManagement>> SearchGroup(string textToSearch)
        {
            //throw new NotImplementedException();
            var group = await _context.GroupManagements
                                    .Include(x => x.GroupFunctions)
                                    .Where(x=>x.GroupId.Contains(textToSearch)||x.GroupName!.Contains(textToSearch)||x.GroupDescription!.Contains(textToSearch))
                                    .ToListAsync();
            return group;
        }
    }

}
