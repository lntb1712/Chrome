using Chrome.DTO;
using Chrome.DTO.FunctionDTO;
using Chrome.DTO.GroupFunctionDTO;
using Chrome.Models;
using Chrome.Repositories.GroupFunctionRepository;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Services.GroupFunctionService
{
    public class GroupFunctionService: IGroupFunctionService
    {
        private readonly IGroupFunctionRepository _groupFunctionRepository;
        private readonly ChromeContext _context;

        public GroupFunctionService(IGroupFunctionRepository groupFunctionRepository,ChromeContext context)
        {
            _groupFunctionRepository = groupFunctionRepository;
            _context = context; 
        }

        public async Task<ServiceResponse<List<FunctionResponseDTO>>> GetAllGroupFunctions()
        {
            //throw new NotImplementedException();
            var lstGroupFuntions = await _groupFunctionRepository.GetFunctionsAsync();
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
            return new ServiceResponse<List<FunctionResponseDTO>>(true,"Lấy danh sách chức năng thành công",lstGroupFunctionsDTO);
        }
        public async Task<ServiceResponse<bool>> DeleteGroupFunction(string groupId, string functionId)
        {
            //throw new NotImplementedException();
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _groupFunctionRepository.DeleteTwoKeyAsync(groupId, functionId);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(false, "Xóa nhóm người dùng thành công", true);
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("reference"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa nhóm người dùng");
                        }
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, "Lỗi không xác định: " + ex.Message);
                }
            }
        }
        public async Task<ServiceResponse<List<GroupFunctionResponseDTO>>> GetGroupFunctionWithGroupID(string groupId)
        {
            //throw new NotImplementedException();
            var lstGroupFunction = await _groupFunctionRepository.GetAllGroupsFunctionWithGroupId(groupId);
            if( lstGroupFunction == null || lstGroupFunction.Count == 0)
            {
                return new ServiceResponse<List<GroupFunctionResponseDTO>>(false, "Không có dữ liệu nhóm chức năng", null!);
            }

            var lstGroup = lstGroupFunction.Select(x => new GroupFunctionResponseDTO
            {
                GroupId = groupId,
                FunctionId = x.FunctionId,
                FunctionName = x.Function.FunctionName!,
                IsEnable = x.IsEnable,
                ApplicableLocation = x.ApplicableLocation!
            }).ToList();
            return new ServiceResponse<List<GroupFunctionResponseDTO>>(true,"Lấy danh sách thành công",lstGroup);
        }
    }
}
