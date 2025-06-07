using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.FunctionRepository
{
    public class FunctionRepository:RepositoryBase<Function>,IFunctionRepository
    {
        private readonly ChromeContext _context;
        public FunctionRepository(ChromeContext context):base(context)
        {
            _context = context;
        }

        public async Task<List<Function>> GetFunctionsAsync()
        {
            //throw new NotImplementedException();
            var functions = await _context.Functions
                                .Include(x => x.GroupFunctions)
                                .ToListAsync();
            return functions;
        }
    }
}
