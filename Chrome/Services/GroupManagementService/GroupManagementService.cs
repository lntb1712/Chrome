using Chrome.DTO;
using Chrome.DTO.GroupManagementDTO;
using Chrome.Models;
using Chrome.Repositories.GroupFunctionRepository;
using Chrome.Repositories.GroupManagementRepository;
using Chrome.Services.GroupManagementService;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.EntityFrameworkCore;

namespace ProductionInventoryManagmentSystem_API.Services.GroupManagementService
{
    public class GroupManagementService:IGroupManagementService
    {
        private readonly IGroupManagementRepository _groupManagementRepository;
        private readonly IGroupFunctionRepository _groupFunctionRepository; 
        private readonly ChromeContext _context;

        public GroupManagementService(IGroupManagementRepository groupManagementRepository, IGroupFunctionRepository groupFunctionRepository, ChromeContext context)
        {
            _groupManagementRepository = groupManagementRepository;
            _groupFunctionRepository = groupFunctionRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddGroupManagement(GroupManagementRequestDTO group)
        {
            //throw new NotImplementedException();\
            if (group == null)
            {
                return new ServiceResponse<bool>(false,"Dữ liệu nhận vào không hợp lệ");
            }

            var groupManagement = new GroupManagement
            {
                GroupId = group.GroupId,
                GroupName = group.GroupName,
                GroupDescription = group.GroupDescription,
                UpdateBy = group.UpdateBy,
                UpdateTime = DateTime.Now,
                GroupFunctions =group.GroupFunctions.Select(x=>new GroupFunction
                {
                    GroupId=group.GroupId,
                    FunctionId=x.FunctionId,
                    IsEnable=x.IsEnable,
                }).ToList()
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
      
                    await _groupManagementRepository.AddAsync(groupManagement,saveChanges:false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm mới nhóm người dùng thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if(error.Contains("unique")||error.Contains("duplicate")||error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Dữ liệu đã tồn tại");
                        }
                        if(error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Dữ liệu tham chiếu không đúng");
                        }

                    }
                    return new ServiceResponse<bool>(false, "Lỗi database:" + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi không xác định: {ex.Message} ");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteGroupManagement(string groupId)
        {
            //throw new NotImplementedException();
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try 
                {
                    await _groupManagementRepository.DeleteAsync(groupId,saveChanges:false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa nhóm người dùng thành công", true);
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
                    return new ServiceResponse<bool>(false,"Lỗi không xác định: "+ ex.Message);
                }
            }
        }

        public async Task<List<GroupManagementResponseDTO>> GetAllGroupManagement()
        {
            //throw new NotImplementedException();
            var lstGroup = await _groupManagementRepository.GetAllGroup();

            var lstGroupManagement = lstGroup.Select(x => new GroupManagementResponseDTO
            {
                GroupId=x.GroupId,
                GroupName=x.GroupName,
                GroupDescription=x.GroupDescription,
                UpdateBy=x.UpdateBy,
                UpdateTime=x.UpdateTime!.Value.ToString("dd-MM-yyyy"),
            }).ToList();
            return lstGroupManagement;
        }

        public async Task<GroupManagementResponseDTO> GetGroupManagementWithGroupId(string groupId)
        {
            //throw new NotImplementedException();
            var groupManagementWithGroupId = await _groupManagementRepository.GetGroupManagementWithGroupID(groupId);
            if (groupManagementWithGroupId == null)
            {
                return null!; // Or throw an exception depending on your requirements
            }

            return new GroupManagementResponseDTO
            {
                GroupId = groupManagementWithGroupId.GroupId,
                GroupName = groupManagementWithGroupId.GroupName,
                GroupDescription = groupManagementWithGroupId.GroupDescription,
                UpdateBy = groupManagementWithGroupId.UpdateBy,
                UpdateTime = groupManagementWithGroupId.UpdateTime.HasValue
                            ? groupManagementWithGroupId.UpdateTime.Value.ToString("dd-MM-yyyy")
                            : null
            };
        }

        public async Task<List<GroupManagementResponseDTO>> SearchGroup(string textToSearch)
        {
            //throw new NotImplementedException();
            var lstSearch = await _groupManagementRepository.SearchGroup(textToSearch);
            var lstGroupManagement = lstSearch.Select(x => new GroupManagementResponseDTO
            {
                GroupId = x.GroupId,
                GroupName = x.GroupName,
                GroupDescription = x.GroupDescription,
                UpdateBy = x.UpdateBy,
                UpdateTime = x.UpdateTime!.Value.ToString("dd-MM-yyyy"),
            }).ToList();
            return lstGroupManagement;
        }

        public async Task<ServiceResponse<bool>> UpdateGroupManagement(GroupManagementRequestDTO group)
        {
            //throw new NotImplementedException();
            if (group == null)
            {
                return new ServiceResponse<bool>(false,"Dữ liệu nhận vào không hợp lệ");
            }    
            var groupManagement = await _groupManagementRepository.GetGroupManagementWithGroupID(group.GroupId);
            if (groupManagement == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy nhóm người dùng hợp lệ");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    groupManagement.GroupName = group.GroupName;
                    groupManagement.GroupDescription = group.GroupDescription;
                    groupManagement.UpdateBy = group.UpdateBy;
                    groupManagement.UpdateTime = DateTime.Now;


                    var existingFunctions = groupManagement.GroupFunctions.ToDictionary(x => x.FunctionId);
                    foreach (var function in group.GroupFunctions)
                    {
                        if(existingFunctions.TryGetValue(function.FunctionId,out var existingFuntion))
                        {
                            existingFuntion.IsEnable=function.IsEnable;
                        }
                        else
                        {
                            groupManagement.GroupFunctions.Add(new GroupFunction
                            {
                                GroupId = group.GroupId,
                                FunctionId = function.FunctionId,
                                IsEnable = function.IsEnable,
                                UpdateBy = function.UpdateBy,
                                UpdateTime = DateTime.Now
                            });
                        }
                    }    

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật thành công", true);
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if(error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Dữ liệu đã tồn tại");
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
    }
}
