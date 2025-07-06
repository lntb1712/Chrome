using Chrome.DTO;
using Chrome.DTO.PutAwayDetailDTO;
using Chrome.DTO.PutAwayDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.Models;
using Chrome.Repositories.PutAwayDetailRepository;
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
        private readonly IPutAwayDetailRepository _putAwayDetailRepository;
        private readonly ChromeContext _context;

        public PutAwayService(IPutAwayRepository putAwayRepository,IPutAwayDetailRepository putAwayDetailRepository, ChromeContext context)
        {
            _putAwayRepository = putAwayRepository ?? throw new ArgumentNullException(nameof(putAwayRepository));
            _putAwayDetailRepository = putAwayDetailRepository ?? throw new ArgumentNullException(nameof(putAwayDetailRepository));
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

        public async Task<ServiceResponse<bool>> UpdatePutAway(PutAwayRequestDTO putAway, IDbContextTransaction transaction = null!)
        {
            bool isExternalTransaction = transaction != null;
            transaction ??= await _context.Database.BeginTransactionAsync();
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
                if (!isExternalTransaction) await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Cập nhật put away thành công");
            }
            catch (DbUpdateException dbEx)
            {
                if (!isExternalTransaction) await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi cập nhật put away: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                if (!isExternalTransaction) await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật put away: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PutAwayAndDetailResponseDTO>> GetPutAwayContainsCodeAsync(string orderCode )
        {
            try
            {
                if (string.IsNullOrEmpty(orderCode))
                {
                    return new ServiceResponse<PutAwayAndDetailResponseDTO>(false, "Mã lệnh không được để trống");
                }

                var putAway = await _putAwayRepository.GetPutAwayContainsCodeAsync(orderCode);
                if (putAway == null)
                {
                    return new ServiceResponse<PutAwayAndDetailResponseDTO>(false, "Put away không tồn tại");
                }

                var putAwayDetail = _putAwayDetailRepository.GetPutAwayDetailsByPutawayNoAsync(putAway.PutAwayCode);
                if (putAwayDetail==null)
                {
                    return new ServiceResponse<PutAwayAndDetailResponseDTO>(false, "Không tìm thấy chi tiết để hàng");
                }

                var response = new PutAwayAndDetailResponseDTO
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
                    PutAwayDescription = putAway.PutAwayDescription,
                    putAwayDetailResponseDTOs = await putAwayDetail.Select(pd => new PutAwayDetailResponseDTO
                    {
                        PutAwayCode = pd.PutAwayCode!,
                        ProductCode = pd.ProductCode!,
                        ProductName = _context.ProductMasters
                            .Where(x => x.ProductCode == pd.ProductCode)
                            .Select(x => x.ProductName)
                            .FirstOrDefault()!,
                        LotNo = pd.LotNo,
                        Demand = pd.Demand,
                        Quantity = pd.Quantity
                    }).ToListAsync()

                };

                return new ServiceResponse<PutAwayAndDetailResponseDTO>(true, "Lấy chi tiết put away thành công", response);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PutAwayAndDetailResponseDTO>(false, $"Lỗi khi lấy chi tiết put away: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<PutAwayResponseDTO>>> GetAllPutAwaysAsyncWithResponsible(string[] warehouseCodes, string responsible, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _putAwayRepository.GetAllPutAwayAsync(warehouseCodes);
                var totalItems = await query.Where(x => x.Responsible == responsible).CountAsync();
                var putAways = await query
                    .Where(x=>x.Responsible==responsible)
                    .OrderBy(x=>x.StatusId)
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

        public async Task<ServiceResponse<PagedResponse<PutAwayResponseDTO>>> SearchPutAwaysAsyncWithResponsible(string[] warehouseCodes, string responsible, string textToSearch, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _putAwayRepository.SearchPutAwayAsync(warehouseCodes, textToSearch);
                var totalItems = await query.Where(x => x.Responsible == responsible).CountAsync();
                var putAways = await query
                    .Where(x=>x.Responsible==responsible)
                    .OrderBy(x => x.StatusId)
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

        public async Task<ServiceResponse<List<PutAwayAndDetailResponseDTO>>> GetListPutAwayContainsCodeAsync(string orderCode)
        {
            try
            {
                if (string.IsNullOrEmpty(orderCode))
                {
                    return new ServiceResponse<List<PutAwayAndDetailResponseDTO>>(false, "Mã lệnh không được để trống");
                }

                var putAways = await _putAwayRepository.GetListGetPutAwayContainsCodeAsync(orderCode);
                if (putAways == null || !putAways.Any())
                {
                    return new ServiceResponse<List<PutAwayAndDetailResponseDTO>>(false, "Không tìm thấy phiếu put away nào");
                }

                var result = new List<PutAwayAndDetailResponseDTO>();

                foreach (var item in putAways)
                {
                    var putAwayDetail = await _putAwayDetailRepository.GetPutAwayDetailsByPutawayNoAsync(item.PutAwayCode).ToListAsync();
                    if (putAwayDetail == null)
                    {
                        return new ServiceResponse<List<PutAwayAndDetailResponseDTO>>(false, $"Không tìm thấy chi tiết để hàng cho mã: {item.PutAwayCode}");
                    }

                    var detailDTOs = await Task.WhenAll(putAwayDetail.Select(async pd => new PutAwayDetailResponseDTO
                    {
                        PutAwayCode = pd.PutAwayCode!,
                        ProductCode = pd.ProductCode!,
                        ProductName = await _context.ProductMasters
                            .Where(x => x.ProductCode == pd.ProductCode)
                            .Select(x => x.ProductName)
                            .FirstOrDefaultAsync() ?? "",
                        LotNo = pd.LotNo,
                        Demand = pd.Demand,
                        Quantity = pd.Quantity
                    }));

                    var dto = new PutAwayAndDetailResponseDTO
                    {
                        PutAwayCode = item.PutAwayCode,
                        OrderTypeCode = item.OrderTypeCode,
                        OrderTypeName = item.OrderTypeCodeNavigation?.OrderTypeName,
                        LocationCode = item.LocationCode,
                        LocationName = item.LocationCodeNavigation?.LocationName,
                        Responsible = item.Responsible,
                        FullNameResponsible = item.ResponsibleNavigation?.FullName,
                        StatusId = item.StatusId,
                        StatusName = item.Status?.StatusName,
                        PutAwayDate = item.PutAwayDate?.ToString("dd/MM/yyyy"),
                        PutAwayDescription = item.PutAwayDescription,
                        putAwayDetailResponseDTOs = detailDTOs.ToList()
                    };

                    result.Add(dto);
                }

                return new ServiceResponse<List<PutAwayAndDetailResponseDTO>>(true, "Lấy danh sách put away thành công", result);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<PutAwayAndDetailResponseDTO>>(false, $"Lỗi khi lấy danh sách put away: {ex.Message}");
            }
        }

    }
}