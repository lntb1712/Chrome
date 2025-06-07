using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.CustomerMasterRepository
{
    public class CustomerMasterRepository:RepositoryBase<CustomerMaster>, ICustomerMasterRepository
    {
        private readonly ChromeContext _context;
        public CustomerMasterRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<CustomerMaster>> GetAllCustomer(int page, int pageSize)
        {
            return await _context.CustomerMasters
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<int> GetTotalCustomerCount()
        {
            return await _context.CustomerMasters.CountAsync();
        }
        public async Task<List<CustomerMaster>> SearchCustomer(string textToSearch, int page, int pageSize)
        {
            return await _context.CustomerMasters
                .Where(c => c.CustomerName!.Contains(textToSearch) || c.CustomerCode.Contains(textToSearch))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<int> GetTotalSearchCount(string textToSearch)
        {
            return await _context.CustomerMasters
                .CountAsync(c => c.CustomerName!.Contains(textToSearch) || c.CustomerCode.Contains(textToSearch));
        }
        public async Task<CustomerMaster> GetCustomerMasterWithCustomerCode(string customerCode)
        {
            var customer = await _context.CustomerMasters
                .FirstOrDefaultAsync(c => c.CustomerCode == customerCode);
            return customer!;
        }
    }
}
