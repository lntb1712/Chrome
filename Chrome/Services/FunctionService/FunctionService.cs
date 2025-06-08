using Chrome.DTO;
using Chrome.DTO.FunctionDTO;
using Chrome.DTO.GroupFunctionDTO;
using Chrome.Repositories.FunctionRepository;
using Chrome.Repositories.WarehouseMasterRepository;

namespace Chrome.Services.FunctionService
{
    public class FunctionService: IFunctionService
    {
        private readonly IFunctionRepository _functionRepository;
        private readonly IWarehouseMasterRepository _warehouseMasterRepository;
        public FunctionService(IFunctionRepository functionRepository, IWarehouseMasterRepository warehouseMasterRepository)
        {
            _functionRepository = functionRepository;
            _warehouseMasterRepository = warehouseMasterRepository;
        }

        public async Task<ServiceResponse<List<FunctionResponseDTO>>> GetAllFunctions()
        {
            //throw new NotImplementedException();
            var lstGroupFuntions = await _functionRepository.GetFunctionsAsync();
            if (lstGroupFuntions == null || lstGroupFuntions.Count == 0)
            {
                return new ServiceResponse<List<FunctionResponseDTO>>(false, "Không có dữ liệu nhóm chức năng", null!);
            }
            var allWarehouses = await _warehouseMasterRepository.GetWarehouseMasters(1,int.MaxValue);
            var lstGroupFunctionsDTO = lstGroupFuntions.Select( row => new FunctionResponseDTO
            {
                FunctionId = row.FunctionId,
                FunctionName = row.FunctionName!,
                IsEnable = false,
                ApplicableLocations = allWarehouses.Select(w => new ApplicableLocationResponseDTO
                {
                    ApplicableLocation = w.WarehouseCode,
                    IsSelected = false
                }).ToList()
            }).ToList();
            return new ServiceResponse<List<FunctionResponseDTO>>(true, "Lấy danh sách chức năng thành công", lstGroupFunctionsDTO);
        }
    }
}
