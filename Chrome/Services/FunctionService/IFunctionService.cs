using Chrome.DTO;
using Chrome.DTO.FunctionDTO;

namespace Chrome.Services.FunctionService
{
    public interface IFunctionService
    {
        Task<ServiceResponse<List<FunctionResponseDTO>>> GetAllFunctions();
    }
}
