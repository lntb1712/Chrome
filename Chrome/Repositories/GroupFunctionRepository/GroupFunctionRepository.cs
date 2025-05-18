using Chrome.Models;
using Chrome.Repositories.GroupFunctionRepository;
using Chrome.Repositories.RepositoryBase;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.EntityFrameworkCore;

namespace ProductionInventoryManagmentSystem_API.Repositories.GroupFunctionRepository
{
    public class GroupFunctionRepository : RepositoryBase<GroupFunction>, IGroupFunctionRepository
    {
        private readonly ChromeContext _context;

        public GroupFunctionRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }


        public async Task<List<GroupFunction>> GetAllGroupsFunctionWithGroupId(string groupId)
        {
            
            var groupFunctions = await _context.GroupFunctions
                                 .Where(row => row.GroupId.Equals(groupId))
                                 .Include(row => row.Function)
                                 .ToListAsync();

            return groupFunctions;
        }

        public async Task<List<Function>> GetFunctionsAsync()
        {
            //throw new NotImplementedException();
            var functions = await _context.Functions
                                .Include(x=>x.GroupFunctions)
                                .ToListAsync();
            return functions;
        }

        public async Task<List<string>> GetListFunctionIDOfGroup(string groupId)
        {
            var lstFunction = await _context.GroupFunctions
                                            .Where(x => x.GroupId == groupId && x.IsEnable == true)
                                            .Select(x => x.FunctionId)
                                            .ToListAsync();
            return lstFunction;
        }
    }
}
