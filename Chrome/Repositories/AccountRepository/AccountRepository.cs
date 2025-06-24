using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;


namespace Chrome.Repositories.AccountRepository
{
    public class AccountRepository: RepositoryBase<AccountManagement>, IAccountRepository
    {
        private readonly ChromeContext _context;
        public AccountRepository(ChromeContext context) :base(context) 
        {
            _context = context;
        }
        public async Task<List<AccountManagement>> GetAllAccount(int page, int pageSize)
        {
            var lstAccount = await _context.AccountManagements
                                           .Include(x=>x.Group)
                                           .ThenInclude(x=>x!.GroupFunctions)
                                           .OrderBy(x=>x.UserName)
                                           .Skip((page-1)*pageSize)
                                           .Take(pageSize)
                                           .ToListAsync();
            return lstAccount;
        }

        public async Task<List<AccountManagement>> GetAllWithRole(string groupID,int page,int pageSize)
        {
            var lstAccountWithRole =await _context.AccountManagements
                                                  .Include(x=>x.Group)
                                                  .ThenInclude(x => x!.GroupFunctions)
                                                  .Where(x=>x.GroupId == groupID)
                                                  .OrderBy(x => x.UserName)
                                                  .Skip((page - 1) * pageSize)
                                                  .Take(pageSize)
                                                  .ToListAsync();
            return lstAccountWithRole;
        }

        public async Task<AccountManagement> GetAccountWithUserName(string userName)
        {
            var lstAccount = await _context.AccountManagements
                                           .Include(x => x.Group)
                                           .ThenInclude(x => x!.GroupFunctions)
                                           .FirstOrDefaultAsync(x=>x.UserName == userName);
            return lstAccount!;
        }

        public async Task<AccountManagement> GetAccountWithUserNameAndPassword(string userName, string password)
        {
            var account = await FindAsync(x => x.UserName == userName && x.Password == password);
            return account;
        }

        public async Task<List<AccountManagement>> SearchAccount(string textToSearch,int page,int pageSize)
        {
            var account = await _context.AccountManagements
                                        .Include(x=>x.Group)
                                        .ThenInclude(x => x!.GroupFunctions)
                                        .Where(x => x.UserName.Contains(textToSearch) || x.FullName!.Contains(textToSearch))
                                        .OrderBy(x => x.UserName)
                                        .Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();
            return account;
        }

        public async Task<int> GetTotalAccountCount()
        {
            return await _context.AccountManagements.CountAsync();
        }

        public async Task<int> GetTotalSearchCount(string textToSearch)
        {
            return await _context.AccountManagements
                                 .Where(x => x.UserName.Contains(textToSearch) || x.FullName!.Contains(textToSearch))
                                 .CountAsync();
        }

        public async Task<int> GetTotalWithRoleCount(string groupID)
        {
            return await _context.AccountManagements
                                 .Where(x => x.GroupId == groupID)
                                 .CountAsync();
        }
    }
}
