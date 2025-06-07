using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.FunctionRepository
{
    public interface IFunctionRepository:IRepositoryBase<Function>
    {
        Task<List<Function>> GetFunctionsAsync();
    }
}
