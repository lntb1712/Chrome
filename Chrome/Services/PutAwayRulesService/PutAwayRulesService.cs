using Chrome.DTO;
using Chrome.DTO.PutAwayRulesDTO;
using Chrome.Models;
using Chrome.Repositories.PutAwayRulesRepository;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Chrome.Services.PutAwayRulesService
{
    public class PutAwayRulesService:IPutAwayRulesService
    {
        private readonly IPutAwayRulesRepository _putAwayRulesRepository;
        private readonly ChromeContext _context;

        public PutAwayRulesService(IPutAwayRulesRepository putAwayRulesRepository, ChromeContext context)
        {
            _putAwayRulesRepository = putAwayRulesRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddPutAwayRule(PutAwayRulesRequestDTO putAwayRuleRequestDTO)
        {
            if (putAwayRuleRequestDTO == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var putAwayRule = new PutAwayRule
            {
                PutAwaysRuleCode = putAwayRuleRequestDTO.PutAwayRuleCode,
                WarehouseToApply = putAwayRuleRequestDTO.WarehouseToApply,
                ProductCode = putAwayRuleRequestDTO.ProductCode,
                LocationCode = putAwayRuleRequestDTO.LocationCode,
                StorageProductId = putAwayRuleRequestDTO.StorageProductId,
            };
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _putAwayRulesRepository.AddAsync(putAwayRule, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm quy tắc sắp xếp hàng hóa thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Dữ liệu đã tồn tại");
                        }
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Dữ liệu tham chiếu không đúng");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi thêm quy tắc sắp xếp hàng hóa: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeletePutAwayRule(string putAwayRuleCode)
        {
            if(string.IsNullOrEmpty(putAwayRuleCode))
            {
                return new ServiceResponse<bool>(false, "Mã quy tắc sắp xếp hàng hóa không hợp lệ");
            }
            var putAwayRule = await _putAwayRulesRepository.GetPutAwayRuleWithCode(putAwayRuleCode);
            if (putAwayRule == null)
            {
                return new ServiceResponse<bool>(false, "Quy tắc sắp xếp hàng hóa không tồn tại");
            }
            using(var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _putAwayRulesRepository.DeleteAsync(putAwayRuleCode, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa quy tắc sắp xếp hàng hóa thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Quy tắc sắp xếp hàng hóa đang được sử dụng và không thể xóa");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa quy tắc sắp xếp hàng hóa: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<PutAwayRulesResponseDTO>>> GetAllPutAwayRules(int page, int pageSize)
        {
            if(page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<PutAwayRulesResponseDTO>>(false, "Trang hoặc kích thước trang không hợp lệ");
            }
            var putAwayRules = await _putAwayRulesRepository.GetAllPutAwayRules(page, pageSize);
            if (putAwayRules == null || !putAwayRules.Any())
            {
                return new ServiceResponse<PagedResponse<PutAwayRulesResponseDTO>>(false, "Không có quy tắc sắp xếp hàng hóa nào được tìm thấy");
            }
            var totalCount = await _putAwayRulesRepository.GetTotalPutAwayRuleCount();
            var putAwayRulesResponse = putAwayRules.Select(rule => new PutAwayRulesResponseDTO 
            {
                PutAwayRuleCode = rule.PutAwaysRuleCode,
                WarehouseToApply = rule.WarehouseToApply,
                WarehouseToApplyName = rule.WarehouseToApplyNavigation?.WarehouseName,
                ProductCode = rule.ProductCode,
                ProductName = rule.ProductCodeNavigation?.ProductName,
                LocationCode = rule.LocationCode,
                LocationName = rule.LocationCodeNavigation?.LocationName,
                StorageProductId = rule.StorageProductId,
                StorageProductName = rule.StorageProduct?.StorageProductName
            }).ToList();
            var pagedResponse = new PagedResponse<PutAwayRulesResponseDTO>(putAwayRulesResponse, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<PutAwayRulesResponseDTO>>(true, "Lấy danh sách quy tắc sắp xếp hàng hóa thành công",pagedResponse);

        }

        public async Task<ServiceResponse<PutAwayRulesResponseDTO>> GetPutAwayRuleWithCode(string putAwayRuleCode)
        {
            if(string.IsNullOrEmpty(putAwayRuleCode))
            {
                return new ServiceResponse<PutAwayRulesResponseDTO>(false, "Mã quy tắc sắp xếp hàng hóa không hợp lệ");
            }
            var putAwayRule = await _putAwayRulesRepository.GetPutAwayRuleWithCode(putAwayRuleCode);
            if (putAwayRule == null)
            {
                return new ServiceResponse<PutAwayRulesResponseDTO>(false, "Quy tắc sắp xếp hàng hóa không tồn tại");
            }
            var putAwayRuleResponse = new PutAwayRulesResponseDTO
            {
                PutAwayRuleCode = putAwayRule.PutAwaysRuleCode,
                WarehouseToApply = putAwayRule.WarehouseToApply,
                WarehouseToApplyName = putAwayRule.WarehouseToApplyNavigation?.WarehouseName,
                ProductCode = putAwayRule.ProductCode,
                ProductName = putAwayRule.ProductCodeNavigation?.ProductName,
                LocationCode = putAwayRule.LocationCode,
                LocationName = putAwayRule.LocationCodeNavigation?.LocationName,
                StorageProductId = putAwayRule.StorageProductId,
                StorageProductName = putAwayRule.StorageProduct?.StorageProductName
            };
            return new ServiceResponse<PutAwayRulesResponseDTO>(true, "Lấy quy tắc sắp xếp hàng hóa thành công", putAwayRuleResponse);
        }

        public async Task<ServiceResponse<int>> GetTotalPutAwayRuleCount()
        {
            try
            {
                var totalCount = await _putAwayRulesRepository.GetTotalPutAwayRuleCount();
                return new ServiceResponse<int>(true, "Lấy tổng số quy tắc sắp xếp hàng hóa thành công", totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, $"Lỗi khi lấy tổng số quy tắc sắp xếp hàng hóa: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<PutAwayRulesResponseDTO>>> SearchPutAwayRules(string textToSearch, int page, int pageSize)
        {
            if(string.IsNullOrEmpty(textToSearch) || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<PutAwayRulesResponseDTO>>(false, "Dữ liệu tìm kiếm không hợp lệ");
            }
            var totalCount = await _putAwayRulesRepository.GetTotalSearchCount(textToSearch);
            if (totalCount == 0)
            {
                return new ServiceResponse<PagedResponse<PutAwayRulesResponseDTO>>(false, "Không tìm thấy quy tắc sắp xếp hàng hóa nào phù hợp với từ khóa tìm kiếm");
            }
            var putAwayRules = await _putAwayRulesRepository.SearchPutAwayRules(textToSearch, page, pageSize);
            if (putAwayRules == null || !putAwayRules.Any())
            {
                return new ServiceResponse<PagedResponse<PutAwayRulesResponseDTO>>(false, "Không tìm thấy quy tắc sắp xếp hàng hóa nào phù hợp với từ khóa tìm kiếm");
            }
            var putAwayRulesResponse = putAwayRules.Select(rule => new PutAwayRulesResponseDTO
            {
                PutAwayRuleCode = rule.PutAwaysRuleCode,
                WarehouseToApply = rule.WarehouseToApply,
                WarehouseToApplyName = rule.WarehouseToApplyNavigation?.WarehouseName,
                ProductCode = rule.ProductCode,
                ProductName = rule.ProductCodeNavigation?.ProductName,
                LocationCode = rule.LocationCode,
                LocationName = rule.LocationCodeNavigation?.LocationName,
                StorageProductId = rule.StorageProductId,
                StorageProductName = rule.StorageProduct?.StorageProductName
            }).ToList();
            var pagedResponse = new PagedResponse<PutAwayRulesResponseDTO>(putAwayRulesResponse, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<PutAwayRulesResponseDTO>>(true, "Tìm kiếm quy tắc sắp xếp hàng hóa thành công", pagedResponse);
        }

        public async Task<ServiceResponse<bool>> UpdatePutAwayRule(PutAwayRulesRequestDTO putAwayRuleRequestDTO)
        {
            if(putAwayRuleRequestDTO == null || string.IsNullOrEmpty(putAwayRuleRequestDTO.PutAwayRuleCode))
            {
                return new ServiceResponse<bool>(false, "Dữ liệu cập nhật không hợp lệ");
            }
            var existingPutAwayRule = await _putAwayRulesRepository.GetPutAwayRuleWithCode(putAwayRuleRequestDTO.PutAwayRuleCode);
            if (existingPutAwayRule == null)
            {
                return new ServiceResponse<bool>(false, "Quy tắc sắp xếp hàng hóa không tồn tại");
            }
            using(var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingPutAwayRule.WarehouseToApply = putAwayRuleRequestDTO.WarehouseToApply;
                    existingPutAwayRule.ProductCode = putAwayRuleRequestDTO.ProductCode;
                    existingPutAwayRule.LocationCode = putAwayRuleRequestDTO.LocationCode;
                    existingPutAwayRule.StorageProductId = putAwayRuleRequestDTO.StorageProductId;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật quy tắc sắp xếp hàng hóa thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Dữ liệu đã tồn tại");
                        }
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Dữ liệu tham chiếu không đúng");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật quy tắc sắp xếp hàng hóa: {ex.Message}");
                }
            }
        }
    }
}
