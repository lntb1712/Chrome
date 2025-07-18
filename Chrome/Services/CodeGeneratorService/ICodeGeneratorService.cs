using Chrome.DTO;

namespace Chrome.Services.CodeGeneratorService
{
    public interface ICodeGeneratorService
    {
        Task<ServiceResponse<string>> GenerateCodeAsync(string warehouseCode,string type);
    }
}
