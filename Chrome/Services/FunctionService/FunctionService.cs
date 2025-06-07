using Chrome.DTO;
using Chrome.DTO.FunctionDTO;
using Chrome.Repositories.FunctionRepository;

namespace Chrome.Services.FunctionService
{
    public class FunctionService: IFunctionService
    {
        private readonly IFunctionRepository _functionRepository;
        public FunctionService(IFunctionRepository functionRepository)
        {
            _functionRepository = functionRepository;
        }

        public async Task<ServiceResponse<List<FunctionResponseDTO>>> GetAllFunctions()
        {
            //throw new NotImplementedException();
            var lstGroupFuntions = await _functionRepository.GetFunctionsAsync();
            if (lstGroupFuntions == null || lstGroupFuntions.Count == 0)
            {
                return new ServiceResponse<List<FunctionResponseDTO>>(false, "Không có dữ liệu nhóm chức năng", null!);
            }

            var lstGroupFunctionsDTO = lstGroupFuntions.Select(row => new FunctionResponseDTO
            {
                FunctionId = row.FunctionId,
                FunctionName = row.FunctionName!,
                IsEnable = false,
                ApplicableLocation = string.Empty,


            }).ToList();
            return new ServiceResponse<List<FunctionResponseDTO>>(true, "Lấy danh sách chức năng thành công", lstGroupFunctionsDTO);
        }
    }
}
