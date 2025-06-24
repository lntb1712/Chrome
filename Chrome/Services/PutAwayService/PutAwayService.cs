using Chrome.DTO;
using Chrome.DTO.PutAwayDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.Models;
using Chrome.Repositories.PutawayRepository;
using Chrome.Repositories.PutAwayRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Chrome.Services.PutAwayService
{
    public class PutAwayService : IPutAwayService
    {
        private readonly IPutAwayRepository _putAwayRepository;
        private readonly ChromeContext _context;

        public PutAwayService(IPutAwayRepository putAwayRepository, ChromeContext context)
        {
            _putAwayRepository = putAwayRepository ?? throw new ArgumentNullException(nameof(putAwayRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ServiceResponse<PagedResponse<PutAwayResponseDTO>>> GetAllPutAwaysAsync(string[] warehouseCodes, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _putAwayRepository.GetAllPutAwayAsync(warehouseCodes);
                var totalItems = await query.CountAsync();
                var putAways = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new PutAwayResponseDTO
                    {
                        PutAwayCode = p.PutAwayCode,
                        OrderTypeCode = p.OrderTypeCode,
                        OrderTypeName = p.OrderTypeCodeNavigation != null ? p.OrderTypeCodeNavigation.OrderTypeName : null,
                        LocationCode = p.LocationCode,
                        LocationName = p.LocationCodeNavigation != null ? p.LocationCodeNavigation.LocationName : null,
                        Responsible = p.Responsible,
                        FullNameResponsible = p.ResponsibleNavigation != null ? p.ResponsibleNavigation.FullName : null,
                        StatusId = p.StatusId,
                        StatusName = p.Status != null ? p.Status.StatusName : null,
                        PutAwayDate = p.PutAwayDate != null ? p.PutAwayDate.Value.ToString("dd/MM/yyyy") : null,
                        PutAwayDescription = p.PutAwayDescription
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<PutAwayResponseDTO>(putAways, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<PutAwayResponseDTO>>(true, "Lấy danh sách put away thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<PutAwayResponseDTO>>(false, $"Lỗi khi lấy danh sách put away: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<PutAwayResponseDTO>>> GetAllPutAwaysWithStatusAsync(string[] warehouseCodes, int statusId, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _putAwayRepository.GetAllPutAwayWithStatus(warehouseCodes, statusId);
                var totalItems = await query.CountAsync();
                var putAways = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new PutAwayResponseDTO
                    {
                        PutAwayCode = p.PutAwayCode,
                        OrderTypeCode = p.OrderTypeCode,
                        OrderTypeName = p.OrderTypeCodeNavigation != null ? p.OrderTypeCodeNavigation.OrderTypeName : null,
                        LocationCode = p.LocationCode,
                        LocationName = p.LocationCodeNavigation != null ? p.LocationCodeNavigation.LocationName : null,
                        Responsible = p.Responsible,
                        FullNameResponsible = p.ResponsibleNavigation != null ? p.ResponsibleNavigation.FullName : null,
                        StatusId = p.StatusId,
                        StatusName = p.Status != null ? p.Status.StatusName : null,
                        PutAwayDate = p.PutAwayDate != null ? p.PutAwayDate.Value.ToString("dd/MM/yyyy") : null,
                        PutAwayDescription = p.PutAwayDescription
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<PutAwayResponseDTO>(putAways, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<PutAwayResponseDTO>>(true, "Lấy danh sách put away theo trạng thái thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<PutAwayResponseDTO>>(false, $"Lỗi khi lấy danh sách put away theo trạng thái: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<PutAwayResponseDTO>>> SearchPutAwaysAsync(string[] warehouseCodes, string textToSearch, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _putAwayRepository.SearchPutAwayAsync(warehouseCodes, textToSearch);
                var totalItems = await query.CountAsync();
                var putAways = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new PutAwayResponseDTO
                    {
                        PutAwayCode = p.PutAwayCode,
                        OrderTypeCode = p.OrderTypeCode,
                        OrderTypeName = p.OrderTypeCodeNavigation != null ? p.OrderTypeCodeNavigation.OrderTypeName : null,
                        LocationCode = p.LocationCode,
                        LocationName = p.LocationCodeNavigation != null ? p.LocationCodeNavigation.LocationName : null,
                        Responsible = p.Responsible,
                        FullNameResponsible = p.ResponsibleNavigation != null ? p.ResponsibleNavigation.FullName : null,
                        StatusId = p.StatusId,
                        StatusName = p.Status != null ? p.Status.StatusName : null,
                        PutAwayDate = p.PutAwayDate != null ? p.PutAwayDate.Value.ToString("dd/MM/yyyy") : null,
                        PutAwayDescription = p.PutAwayDescription
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<PutAwayResponseDTO>(putAways, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<PutAwayResponseDTO>>(true, "Tìm kiếm put away thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<PutAwayResponseDTO>>(false, $"Lỗi khi tìm kiếm put away: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PutAwayResponseDTO>> GetPutAwayByCodeAsync(string putAwayCode)
        {
            try
            {
                if (string.IsNullOrEmpty(putAwayCode))
                {
                    return new ServiceResponse<PutAwayResponseDTO>(false, "Mã put away không được để trống");
                }

                var putAway = await _putAwayRepository.GetPutAwayCodeAsync(putAwayCode);
                if (putAway == null)
                {
                    return new ServiceResponse<PutAwayResponseDTO>(false, "Put away không tồn tại");
                }

                var response = new PutAwayResponseDTO
                {
                    PutAwayCode = putAway.PutAwayCode,
                    OrderTypeCode = putAway.OrderTypeCode,
                    OrderTypeName = putAway.OrderTypeCodeNavigation != null ? putAway.OrderTypeCodeNavigation.OrderTypeName : null,
                    LocationCode = putAway.LocationCode,
                    LocationName = putAway.LocationCodeNavigation != null ? putAway.LocationCodeNavigation.LocationName : null,
                    Responsible = putAway.Responsible,
                    FullNameResponsible = putAway.ResponsibleNavigation != null ? putAway.ResponsibleNavigation.FullName : null,
                    StatusId = putAway.StatusId,
                    StatusName = putAway.Status != null ? putAway.Status.StatusName : null,
                    PutAwayDate = putAway.PutAwayDate != null ? putAway.PutAwayDate.Value.ToString("dd/MM/yyyy") : null,
                    PutAwayDescription = putAway.PutAwayDescription
                };

                return new ServiceResponse<PutAwayResponseDTO>(true, "Lấy chi tiết put away thành công", response);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PutAwayResponseDTO>(false, $"Lỗi khi lấy chi tiết put away: {ex.Message}");
            }
        }
        public async Task<ServiceResponse<List<StatusMasterResponseDTO>>> GetListStatusMaster()
        {
            try
            {
                var statuses = await _context.StatusMasters
                    .Select(x => new StatusMasterResponseDTO
                    {
                        StatusId = x.StatusId,
                        StatusName = x.StatusName
                    })
                    .ToListAsync();

                return new ServiceResponse<List<StatusMasterResponseDTO>>(true, "Lấy danh sách StatusMaster thành công", statuses);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StatusMasterResponseDTO>>(false, $"Lỗi khi lấy danh sách StatusMaster: {ex.Message}");
            }
        }
        public async Task<ServiceResponse<bool>> AddPutAway(PutAwayRequestDTO putAway, IDbContextTransaction transaction = null!)
        {
            bool isExternalTransaction = transaction != null;
            transaction ??= await _context.Database.BeginTransactionAsync();
            try
            {
                if (putAway == null || string.IsNullOrEmpty(putAway.PutAwayCode))
                {
                    return new ServiceResponse<bool>(false, "Dữ liệu đầu vào không hợp lệ. PutAwayCode là bắt buộc.");
                }

                // Kiểm tra PutAwayCode đã tồn tại chưa
                var existingPutAway = await _putAwayRepository.GetPutAwayCodeAsync(putAway.PutAwayCode);
                if (existingPutAway != null)
                {
                    return new ServiceResponse<bool>(false, "Put away với mã này đã tồn tại.");
                }

                // Kiểm tra OrderTypeCode hợp lệ (nếu có)
                if (!string.IsNullOrEmpty(putAway.OrderTypeCode))
                {
                    var orderType = await _context.OrderTypes
                        .FirstOrDefaultAsync(ot => ot.OrderTypeCode == putAway.OrderTypeCode);
                    if (orderType == null)
                    {
                        return new ServiceResponse<bool>(false, "OrderTypeCode không tồn tại.");
                    }
                }

                // Kiểm tra LocationCode hợp lệ (nếu có)
                if (!string.IsNullOrEmpty(putAway.LocationCode))
                {
                    var location = await _context.LocationMasters
                        .FirstOrDefaultAsync(l => l.LocationCode == putAway.LocationCode);
                    if (location == null)
                    {
                        return new ServiceResponse<bool>(false, "LocationCode không tồn tại.");
                    }
                }

                // Kiểm tra Responsible hợp lệ (nếu có)
                if (!string.IsNullOrEmpty(putAway.Responsible))
                {
                    var user = await _context.AccountManagements
                        .FirstOrDefaultAsync(u => u.UserName == putAway.Responsible);
                    if (user == null)
                    {
                        return new ServiceResponse<bool>(false, "Responsible không tồn tại.");
                    }
                }

                // Tạo mới PutAway
                var newPutAway = new PutAway
                {
                    PutAwayCode = putAway.PutAwayCode,
                    OrderTypeCode = putAway.OrderTypeCode,
                    LocationCode = putAway.LocationCode,
                    Responsible = putAway.Responsible,
                    StatusId = putAway.StatusId ?? 1, // Giả định trạng thái mặc định là 1 (Chưa hoàn thành)
                    PutAwayDate = !string.IsNullOrEmpty(putAway.PutAwayDate) && DateTime.TryParseExact(putAway.PutAwayDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var parsedDate)
                        ? parsedDate
                        : DateTime.Now,
                    PutAwayDescription = putAway.PutAwayDescription
                };

                await _putAwayRepository.AddAsync(newPutAway, saveChanges: false);
                await _context.SaveChangesAsync();
                if (!isExternalTransaction) await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Thêm put away thành công");
            }
            catch (DbUpdateException dbEx)
            {
                if (!isExternalTransaction) await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi thêm put away: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                if (!isExternalTransaction) await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi thêm put away: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> DeletePutAway(string putAwayCode)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(putAwayCode))
                {
                    return new ServiceResponse<bool>(false, "Mã put away không được để trống.");
                }

                var putAway = await _putAwayRepository.GetPutAwayCodeAsync(putAwayCode);
                if (putAway == null)
                {
                    return new ServiceResponse<bool>(false, "Put away không tồn tại.");
                }

                await _putAwayRepository.DeleteAsync(putAwayCode, saveChanges: false);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Xóa put away thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi xóa put away: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi xóa put away: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> UpdatePutAway(PutAwayRequestDTO putAway)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (putAway == null || string.IsNullOrEmpty(putAway.PutAwayCode))
                {
                    return new ServiceResponse<bool>(false, "Dữ liệu đầu vào không hợp lệ. PutAwayCode là bắt buộc.");
                }

                var existingPutAway = await _putAwayRepository.GetPutAwayCodeAsync(putAway.PutAwayCode);
                if (existingPutAway == null)
                {
                    return new ServiceResponse<bool>(false, "Put away không tồn tại.");
                }

                // Kiểm tra OrderTypeCode hợp lệ (nếu có)
                if (!string.IsNullOrEmpty(putAway.OrderTypeCode))
                {
                    var orderType = await _context.OrderTypes
                        .FirstOrDefaultAsync(ot => ot.OrderTypeCode == putAway.OrderTypeCode);
                    if (orderType == null)
                    {
                        return new ServiceResponse<bool>(false, "OrderTypeCode không tồn tại.");
                    }
                    existingPutAway.OrderTypeCode = putAway.OrderTypeCode;
                }

                // Kiểm tra LocationCode hợp lệ (nếu có)
                if (!string.IsNullOrEmpty(putAway.LocationCode))
                {
                    var location = await _context.LocationMasters
                        .FirstOrDefaultAsync(l => l.LocationCode == putAway.LocationCode);
                    if (location == null)
                    {
                        return new ServiceResponse<bool>(false, "LocationCode không tồn tại.");
                    }
                    existingPutAway.LocationCode = putAway.LocationCode;
                }

                // Kiểm tra Responsible hợp lệ (nếu có)
                if (!string.IsNullOrEmpty(putAway.Responsible))
                {
                    var user = await _context.AccountManagements
                        .FirstOrDefaultAsync(u => u.UserName == putAway.Responsible);
                    if (user == null)
                    {
                        return new ServiceResponse<bool>(false, "Responsible không tồn tại.");
                    }
                    existingPutAway.Responsible = putAway.Responsible;
                }

                // Cập nhật các thuộc tính khác
                existingPutAway.StatusId = putAway.StatusId ?? existingPutAway.StatusId;
                existingPutAway.PutAwayDescription = putAway.PutAwayDescription ?? existingPutAway.PutAwayDescription;
                if (!string.IsNullOrEmpty(putAway.PutAwayDate) && DateTime.TryParseExact(putAway.PutAwayDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
                {
                    existingPutAway.PutAwayDate = parsedDate;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Cập nhật put away thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi cập nhật put away: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật put away: {ex.Message}");
            }
        }
    }
}