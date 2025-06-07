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
            }).ToList();
            return new ServiceResponse<List<GroupFunctionResponseDTO>>(true,"Lấy danh sách thành công",lstGroup);
        }

        public async Task<ServiceResponse<List<ApplicableLocationResponseDTO>>> GetListApplicableSelected()
        {
            var lstApplicableLocation = await _groupFunctionRepository.GetListApplicableSelected();
            if( lstApplicableLocation == null || lstApplicableLocation.Count==0)
            {
                return new ServiceResponse<List<ApplicableLocationResponseDTO>>(false, "Không có dữ liệu kho", null!);
            }
            var lstApplicableLocationResponse = lstApplicableLocation.Select(x => new ApplicableLocationResponseDTO
            {
                ApplicableLocation = x.ApplicableLocation,
                IsSelected = x.IsSelected
            }).ToList();
            return new ServiceResponse<List<ApplicableLocationResponseDTO>>(true, "Lấy danh sách kho thành công", lstApplicableLocationResponse);


        }
    }
}
