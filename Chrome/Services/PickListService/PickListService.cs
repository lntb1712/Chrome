using Chrome.DTO;
using Chrome.DTO.PickListDetailDTO;
using Chrome.DTO.PickListDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.Models;
using Chrome.Repositories.PickListDetailRepository;
using Chrome.Repositories.PickListRepository;
using Chrome.Services.ReservationService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Chrome.Services.PickListService
{
    public class PickListService : IPickListService
    {
        private readonly IPickListRepository _pickListRepository;
        private readonly IPickListDetailRepository _pickListDetailRepository;
        private readonly ChromeContext _context;

        public PickListService(IPickListRepository pickListRepository,IPickListDetailRepository pickListDetailRepository, ChromeContext context)
        {
            _pickListRepository = pickListRepository ?? throw new ArgumentNullException(nameof(pickListRepository));
            _pickListDetailRepository = pickListDetailRepository ?? throw new ArgumentNullException(nameof(pickListDetailRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ServiceResponse<PagedResponse<PickListResponseDTO>>> GetAllPickListsAsync(string[] warehouseCodes, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _pickListRepository.GetAllPickListAsync(warehouseCodes);
                var totalItems = await query.CountAsync();
                var pickLists = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new PickListResponseDTO
                    {
                        PickNo = p.PickNo,
                        ReservationCode = p.ReservationCode,
                        WarehouseCode = p.WarehouseCode,
                        WarehouseName = p.WarehouseCodeNavigation!.WarehouseName,
                        Responsible = p.Responsible,
                        FullNameResponsible = p.ResponsibleNavigation!.FullName,
                        PickDate = p.PickDate!.Value.ToString("dd/MM/yyyy"),
                        StatusId = p.StatusId,
                        StatusName = p.Status!.StatusName
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<PickListResponseDTO>(pickLists, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<PickListResponseDTO>>(true, "Lấy danh sách pick list thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<PickListResponseDTO>>(false, $"Lỗi khi lấy danh sách pick list: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<PickListResponseDTO>>> GetAllPickListsWithStatusAsync(string[] warehouseCodes, int statusId, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _pickListRepository.GetAllPickListWithStatus(warehouseCodes, statusId);
                var totalItems = await query.CountAsync();
                var pickLists = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new PickListResponseDTO
                    {
                        PickNo = p.PickNo,
                        ReservationCode = p.ReservationCode,
                        WarehouseCode = p.WarehouseCode,
                        WarehouseName = p.WarehouseCodeNavigation!.WarehouseName,
                        Responsible = p.Responsible,
                        FullNameResponsible = p.ResponsibleNavigation!.FullName,
                        PickDate = p.PickDate!.Value.ToString("dd/MM/yyyy"),
                        StatusId = p.StatusId,
                        StatusName = p.Status!.StatusName
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<PickListResponseDTO>(pickLists, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<PickListResponseDTO>>(true, "Lấy danh sách pick list theo trạng thái thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<PickListResponseDTO>>(false, $"Lỗi khi lấy danh sách pick list theo trạng thái: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<PickListResponseDTO>>> SearchPickListsAsync(string[] warehouseCodes, string textToSearch, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _pickListRepository.SearchPickListAsync(warehouseCodes, textToSearch);
                var totalItems = await query.CountAsync();
                var pickLists = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new PickListResponseDTO
                    {
                        PickNo = p.PickNo,
                        ReservationCode = p.ReservationCode,
                        WarehouseCode = p.WarehouseCode,
                        WarehouseName = p.WarehouseCodeNavigation!.WarehouseName,
                        PickDate = p.PickDate!.Value.ToString("dd/MM/yyyy"),
                        StatusId = p.StatusId,
                        StatusName = p.Status!.StatusName
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<PickListResponseDTO>(pickLists, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<PickListResponseDTO>>(true, "Tìm kiếm pick list thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<PickListResponseDTO>>(false, $"Lỗi khi tìm kiếm pick list: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PickListResponseDTO>> GetPickListByCodeAsync(string pickNo)
        {
            try
            {
                if (string.IsNullOrEmpty(pickNo))
                {
                    return new ServiceResponse<PickListResponseDTO>(false, "Mã pick list không được để trống");
                }

                var pickList = await _pickListRepository.GetPickListWithCode(pickNo);
                if (pickList == null)
                {
                    return new ServiceResponse<PickListResponseDTO>(false, "Pick list không tồn tại");
                }

                var response = new PickListResponseDTO
                {
                    PickNo = pickList.PickNo,
                    ReservationCode = pickList.ReservationCode,
                    WarehouseCode = pickList.WarehouseCode,
                    WarehouseName = pickList.WarehouseCodeNavigation!.WarehouseName,
                    Responsible= pickList.Responsible,
                    FullNameResponsible = pickList.ResponsibleNavigation!.FullName,
                    PickDate = pickList.PickDate!.Value.ToString("dd/MM/yyyy"),
                    StatusId = pickList.StatusId,
                    StatusName = pickList.Status!.StatusName
                };

                return new ServiceResponse<PickListResponseDTO>(true, "Lấy chi tiết pick list thành công", response);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PickListResponseDTO>(false, $"Lỗi khi lấy chi tiết pick list: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> AddPickList(PickListRequestDTO pickList, IDbContextTransaction transaction = null!)
        {
            bool isExternalTransaction = transaction != null;
            transaction ??= await _context.Database.BeginTransactionAsync();
           
            try
            {
                if (pickList == null || string.IsNullOrEmpty(pickList.PickNo) || string.IsNullOrEmpty(pickList.WarehouseCode))
                {
                    return new ServiceResponse<bool>(false, "Dữ liệu đầu vào không hợp lệ. PickNo và WarehouseCode là bắt buộc.");
                }

                // Kiểm tra PickNo đã tồn tại chưa
                var existingPickList = await _pickListRepository.GetPickListWithCode(pickList.PickNo);
                if (existingPickList != null)
                {
                    return new ServiceResponse<bool>(false, "Pick list với mã này đã tồn tại.");
                }

                // Kiểm tra ReservationCode hợp lệ
                if (string.IsNullOrEmpty(pickList.ReservationCode))
                {
                    return new ServiceResponse<bool>(false, "ReservationCode là bắt buộc.");
                }

                var reservation = await _context.Reservations
                    .Include(r => r.ReservationDetails)
                    .FirstOrDefaultAsync(r => r.ReservationCode == pickList.ReservationCode);
                if (reservation == null)
                {
                    return new ServiceResponse<bool>(false, "ReservationCode không tồn tại.");
                }

                // Tạo mới PickList
                var newPickList = new PickList
                {
                    PickNo = pickList.PickNo,
                    ReservationCode = pickList.ReservationCode,
                    WarehouseCode = pickList.WarehouseCode,
                    Responsible = pickList.Responsible,
                    PickDate = DateTime.Now,
                    StatusId = 1 // Giả định trạng thái mặc định là 1 (Chưa hoàn thành)
                };

                await _pickListRepository.AddAsync(newPickList, saveChanges: false);

                // Tạo PickListDetail dựa trên ReservationDetail (chỉ chèn dữ liệu, không kiểm tra)
                var reservationDetails = reservation.ReservationDetails
                    .Where(rd => rd.QuantityReserved > 0)
                    .ToList();

                if (!reservationDetails.Any())
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, "Không có ReservationDetail để tạo PickListDetail.");
                }

                foreach (var reservationDetail in reservationDetails)
                {
                    var pickListDetail = new PickListDetail
                    {
                        PickNo = pickList.PickNo,
                        ProductCode = reservationDetail.ProductCode!,
                        LotNo = reservationDetail.LotNo!,
                        Demand = (float)reservationDetail.QuantityReserved!, // Lấy QuantityReserved làm Demand
                        Quantity = 0, // Mặc định Quantity là 0 theo bảng
                        LocationCode = reservationDetail.LocationCode
                    };

                    await _context.PickListDetails.AddAsync(pickListDetail);
                }

                await _context.SaveChangesAsync();
                if (!isExternalTransaction) await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Thêm pick list và chi tiết thành công");
            }
            catch (DbUpdateException dbEx)
            {
                if (!isExternalTransaction) await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi thêm pick list: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                if (!isExternalTransaction) await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi thêm pick list: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> DeletePickList(string pickNo, IDbContextTransaction transaction = null!)
        {
            bool isExternalTransaction = transaction != null;
            transaction ??= await _context.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(pickNo))
                {
                    return new ServiceResponse<bool>(false, "Mã pick list không được để trống.");
                }

                var pickList = await _pickListRepository.GetPickListWithCode(pickNo);
                if (pickList == null)
                {
                    return new ServiceResponse<bool>(false, "Pick list không tồn tại.");
                }
                var pickListDetails = await _context.PickListDetails
                                     .Where(p => p.PickNo == pickNo)
                                     .ToListAsync();

                _context.PickListDetails.RemoveRange(pickListDetails);

                await _context.SaveChangesAsync();
                await _pickListRepository.DeleteAsync(pickNo,saveChanges:false);
                await _context.SaveChangesAsync();
                if (!isExternalTransaction) await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Xóa pick list thành công");
            }
            catch (DbUpdateException dbEx)
            {
                if (!isExternalTransaction) await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi xóa pick list: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                if (!isExternalTransaction) await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi xóa pick list: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> UpdatePickList(PickListRequestDTO pickList)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (pickList == null || string.IsNullOrEmpty(pickList.PickNo) || string.IsNullOrEmpty(pickList.WarehouseCode))
                {
                    return new ServiceResponse<bool>(false, "Dữ liệu đầu vào không hợp lệ. PickNo và WarehouseCode là bắt buộc.");
                }

                var existingPickList = await _pickListRepository.GetPickListWithCode(pickList.PickNo);
                if (existingPickList == null)
                {
                    return new ServiceResponse<bool>(false, "Pick list không tồn tại.");
                }

                // Kiểm tra ReservationCode hợp lệ (nếu có)
                if (!string.IsNullOrEmpty(pickList.ReservationCode))
                {
                    var reservation = await _context.Reservations
                        .FirstOrDefaultAsync(r => r.ReservationCode == pickList.ReservationCode);
                    if (reservation == null)
                    {
                        return new ServiceResponse<bool>(false, "ReservationCode không tồn tại.");
                    }
                }

                existingPickList.StatusId = pickList.StatusId ?? existingPickList.StatusId;


                // Kiểm tra và cập nhật trạng thái Reservation nếu PickList hoàn thành (StatusID = 3)
                if (pickList.StatusId == 3 && !string.IsNullOrEmpty(existingPickList.ReservationCode))
                {
                    var relatedReservation = await _context.Reservations
                        .FirstOrDefaultAsync(r => r.ReservationCode == existingPickList.ReservationCode);
                    if (relatedReservation != null && relatedReservation.StatusId != 3)
                    {
                        relatedReservation.StatusId = 3; // Cập nhật trạng thái Reservation thành hoàn thành
                        _context.Reservations.Update(relatedReservation);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Cập nhật pick list thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi cập nhật pick list: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật pick list: {ex.Message}");
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

        public async Task<ServiceResponse<PickAndDetailResponseDTO>> GetPickListContainCodeAsync(string orderCode)
        {
            try
            {
                if (string.IsNullOrEmpty(orderCode))
                {
                    return new ServiceResponse<PickAndDetailResponseDTO>(false, "Mã lệnh không được để trống");
                }

                var pickList = await _pickListRepository.GetPickListContainCode(orderCode);
                if (pickList == null)
                {
                    return new ServiceResponse<PickAndDetailResponseDTO>(false, "Pick list không tồn tại");
                }
                var pickListDetail =  _pickListDetailRepository.GetPickListDetailsByPickNoAsync(pickList.PickNo);
                if (pickListDetail == null)
                {
                    return new ServiceResponse<PickAndDetailResponseDTO>(false, "Chi tiết phiếu lấy không tồn tại");
                }

                var response = new PickAndDetailResponseDTO
                {
                    PickNo = pickList.PickNo,
                    ReservationCode = pickList.ReservationCode,
                    WarehouseCode = pickList.WarehouseCode,
                    WarehouseName = pickList.WarehouseCodeNavigation!.WarehouseName,
                    Responsible = pickList.Responsible,
                    FullNameResponsible = pickList.ResponsibleNavigation!.FullName,
                    PickDate = pickList.PickDate!.Value.ToString("dd/MM/yyyy"),
                    StatusId = pickList.StatusId,
                    StatusName = pickList.Status!.StatusName,
                    pickListDetailResponseDTOs = await pickListDetail.Select(pd => new PickListDetailResponseDTO
                    {
                        PickNo = pd.PickNo!,
                        ProductCode = pd.ProductCode!,
                        ProductName = _context.ProductMasters
                            .Where(x => x.ProductCode == pd.ProductCode)
                            .Select(x => x.ProductName)
                            .FirstOrDefault(), // Fix: Extract ProductName as string
                        LotNo = pd.LotNo,
                        Demand = pd.Demand,
                        Quantity = pd.Quantity,
                        LocationCode = pd.LocationCode,
                        LocationName = _context.LocationMasters
                                               .Where(x => x.LocationCode == pd.LocationCode)
                                               .Select(x => x.LocationName)
                                               .FirstOrDefault()
                    }).ToListAsync()

                };

                return new ServiceResponse<PickAndDetailResponseDTO>(true, "Lấy  pick list thành công", response);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PickAndDetailResponseDTO>(false, $"Lỗi khi lấy  pick list: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<PickListResponseDTO>>> GetAllPickListsAsyncWithResponsible(string[] warehouseCodes, string responsible, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query =  _pickListRepository.GetAllPickListAsync(warehouseCodes);
                                                     
                var totalItems = await query.Where(x => x.Responsible == responsible).CountAsync();
                var pickLists = await query.Where(x => x.Responsible == responsible)
                    .Select(p => new PickListResponseDTO
                    {
                        PickNo = p.PickNo,
                        ReservationCode = p.ReservationCode,
                        WarehouseCode = p.WarehouseCode,
                        WarehouseName = p.WarehouseCodeNavigation!.WarehouseName,
                        Responsible = p.Responsible,
                        FullNameResponsible = p.ResponsibleNavigation!.FullName,
                        PickDate = p.PickDate!.Value.ToString("dd/MM/yyyy"),
                        StatusId = p.StatusId,
                        StatusName = p.Status!.StatusName
                    })
                    .OrderBy(x => x.StatusId)
                    .ToListAsync();

                var pagedResponse = new PagedResponse<PickListResponseDTO>(pickLists, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<PickListResponseDTO>>(true, "Lấy danh sách pick list thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<PickListResponseDTO>>(false, $"Lỗi khi lấy danh sách pick list: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<PickListResponseDTO>>> SearchPickListsAsyncWithResponsible(string[] warehouseCodes, string responsible, string textToSearch, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _pickListRepository.SearchPickListAsync(warehouseCodes, textToSearch);
                var totalItems = await query.Where(x => x.Responsible == responsible).CountAsync();
                var pickLists = await query
                    .Where(x=>x.Responsible==responsible)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new PickListResponseDTO
                    {
                        PickNo = p.PickNo,
                        ReservationCode = p.ReservationCode,
                        WarehouseCode = p.WarehouseCode,
                        WarehouseName = p.WarehouseCodeNavigation!.WarehouseName,
                        PickDate = p.PickDate!.Value.ToString("dd/MM/yyyy"),
                        StatusId = p.StatusId,
                        StatusName = p.Status!.StatusName
                    })
                    .OrderBy(x=>x.StatusId)
                    .ToListAsync();

                var pagedResponse = new PagedResponse<PickListResponseDTO>(pickLists, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<PickListResponseDTO>>(true, "Tìm kiếm pick list thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<PickListResponseDTO>>(false, $"Lỗi khi tìm kiếm pick list: {ex.Message}");
            }
        }
    }
}