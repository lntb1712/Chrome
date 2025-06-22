using Chrome.DTO;
using Chrome.DTO.OrderDetailBaseDTO;
using Chrome.DTO.OrderTypeDTO;
using Chrome.DTO.ReservationDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.DTO.WarehouseMasterDTO;
using Chrome.Models;
using Chrome.Repositories.InventoryRepository;
using Chrome.Repositories.ReservationRepository;
using Chrome.Services.InventoryService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Chrome.Services.ReservationService
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ChromeContext _context;

        public ReservationService(IReservationRepository reservationRepository, IInventoryRepository inventoryRepository, ChromeContext context)
        {
            _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
            _inventoryRepository = inventoryRepository;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ServiceResponse<PagedResponse<ReservationResponseDTO>>> GetAllReservations(string[] warehouseCodes, int page, int pageSize)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _reservationRepository.GetAllReservationsAsync(warehouseCodes);
                var totalItems = await query.CountAsync();
                var reservations = await query
                    .Select(x => new ReservationResponseDTO
                    {
                        ReservationCode = x.ReservationCode,
                        OrderTypeCode = x.OrderTypeCode,
                        OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                        OrderId = x.OrderId,
                        ReservationDate = x.ReservationDate!.Value.ToString("dd/MM/yyyy"),
                        WarehouseCode = x.WarehouseCode,
                        WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                        StatusId = x.StatusId,
                        StatusName = x.Status!.StatusName
                    })
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var pagedResponse = new PagedResponse<ReservationResponseDTO>(reservations, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<ReservationResponseDTO>>(true, "Lấy danh sách reservation thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<ReservationResponseDTO>>(false, $"Lỗi khi lấy danh sách reservation: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<ReservationResponseDTO>>> GetAllReservationsWithStatus(string[] warehouseCodes, int statusId, int page, int pageSize)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _reservationRepository.GetAllReservationsWithStatus(warehouseCodes, statusId);
                var totalItems = await query.CountAsync();
                var reservations = await query
                    .Select(x => new ReservationResponseDTO
                    {
                        ReservationCode = x.ReservationCode,
                        OrderTypeCode = x.OrderTypeCode,
                        OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                        OrderId = x.OrderId,
                        ReservationDate = x.ReservationDate!.Value.ToString("dd/MM/yyyy"),
                        WarehouseCode = x.WarehouseCode,
                        WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                        StatusId = x.StatusId,
                        StatusName = x.Status!.StatusName
                    })
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var pagedResponse = new PagedResponse<ReservationResponseDTO>(reservations, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<ReservationResponseDTO>>(true, "Lấy danh sách reservation theo trạng thái thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<ReservationResponseDTO>>(false, $"Lỗi khi lấy danh sách reservation: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<ReservationResponseDTO>>> SearchReservationsAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _reservationRepository.SearchReservationsAsync(warehouseCodes, textToSearch);
                var totalItems = await query.CountAsync();
                var reservations = await query
                    .Select(x => new ReservationResponseDTO
                    {
                        ReservationCode = x.ReservationCode,
                        OrderTypeCode = x.OrderTypeCode,
                        OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                        OrderId = x.OrderId,
                        ReservationDate = x.ReservationDate!.Value.ToString("dd/MM/yyyy"),
                        WarehouseCode = x.WarehouseCode,
                        WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                        StatusId = x.StatusId,
                        StatusName = x.Status!.StatusName
                    })
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var pagedResponse = new PagedResponse<ReservationResponseDTO>(reservations, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<ReservationResponseDTO>>(true, "Tìm kiếm reservation thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<ReservationResponseDTO>>(false, $"Lỗi khi tìm kiếm reservation: {ex.Message}");
            }
        }
        public async Task<ServiceResponse<bool>> AddReservation(ReservationRequestDTO reservation)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            string[] formats = { "M/d/yyyy h:mm:ss tt", "MM/dd/yyyy hh:mm:ss tt", "dd/MM/yyyy" };
            if (!DateTime.TryParseExact(reservation.ReservationDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return new ServiceResponse<bool>(false, "Ngày giữ hàng không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }

            try
            {
                // Xác định loại OrderId (StockOut) dựa trên OrderTypeCode
                var orderDetails = new List<OrderDetailBaseDTO>();
                if (reservation.OrderTypeCode == "SO" || reservation.OrderTypeCode!.StartsWith("SO")) // Giả định "SO" là StockOut
                {
                    orderDetails = await _context.StockOutDetails
                        .Where(od => od.StockOutCode == reservation.OrderId)
                        .Select(od => new OrderDetailBaseDTO
                        {
                            ProductCode = od.ProductCode,
                            Quantity = od.Demand ?? 0
                        })
                        .ToListAsync();
                }
                if (reservation.OrderTypeCode == "MO" || reservation.OrderTypeCode!.StartsWith("MO")) // Giả định "SO" là StockOut
                {
                    orderDetails = await _context.MovementDetails
                        .Where(od => od.MovementCode == reservation.OrderId)
                        .Select(od => new OrderDetailBaseDTO
                        {
                            ProductCode = od.ProductCode,
                            Quantity = od.Demand ?? 0
                        })
                        .ToListAsync();
                }
                if (reservation.OrderTypeCode == "TF" || reservation.OrderTypeCode!.StartsWith("TF")) // Giả định "SO" là StockOut
                {
                    orderDetails = await _context.TransferDetails
                        .Where(od => od.TransferCode == reservation.OrderId)
                        .Select(od => new OrderDetailBaseDTO
                        {
                            ProductCode = od.ProductCode,
                            Quantity = od.Demand ?? 0
                        })
                        .ToListAsync();
                }
                else
                {
                    return new ServiceResponse<bool>(false, "Loại OrderId không được hỗ trợ.");
                }

                if (!orderDetails.Any())
                {
                    return new ServiceResponse<bool>(false, "Không tìm thấy chi tiết đơn hàng để tạo ReservationDetail.");
                }

                // Kiểm tra tồn kho và reservation hiện có trước khi tạo, bỏ qua status = 3
                foreach (var orderDetail in orderDetails)
                {
                    double totalRequiredQuantity = orderDetail.Quantity;
                    if (totalRequiredQuantity <= 0)
                        continue;

                    var inventoryItems = await _inventoryRepository.GetInventoryByProductCodeAsync(orderDetail.ProductCode, reservation.WarehouseCode!)
                        .OrderBy(x => x.ReceiveDate) // FIFO dựa trên ngày nhận hàng
                        .ToListAsync();

                    if (!inventoryItems.Any())
                    {
                        return new ServiceResponse<bool>(false, $"Không có tồn kho cho sản phẩm {orderDetail.ProductCode}");
                    }

                    double remainingQuantityToCheck = totalRequiredQuantity;
                    foreach (var inventory in inventoryItems)
                    {
                        // Tính tồn kho khả dụng sau khi trừ reservation hiện có, chỉ tính các reservation chưa hoàn thành (StatusId != 3)
                        var reservedQuantity = await _context.ReservationDetails
                            .Include(rd => rd.ReservationCodeNavigation) // Tải thông tin Reservation liên quan
                            .Where(rd => rd.ProductCode == inventory.ProductCode &&
                                         rd.Lotno == inventory.Lotno &&
                                         rd.LocationCode == inventory.LocationCode &&
                                         rd.ReservationCodeNavigation!.StatusId != 3)
                            .SumAsync(rd => (double?)rd.QuantityReserved) ?? 0;

                        double availableQuantity = (inventory.Quantity ?? 0) - reservedQuantity;
                        if (availableQuantity <= 0)
                            continue;

                        remainingQuantityToCheck -= availableQuantity;
                        if (remainingQuantityToCheck <= 0)
                            break;
                    }

                    if (remainingQuantityToCheck > 0)
                    {
                        return new ServiceResponse<bool>(false, $"Không đủ tồn kho khả dụng cho sản phẩm {orderDetail.ProductCode} sau khi trừ reservation chưa hoàn thành");
                    }
                }

                // Nếu kiểm tra qua, tạo Reservation
                var entity = new Reservation
                {
                    ReservationCode = reservation.ReservationCode,
                    OrderTypeCode = reservation.OrderTypeCode,
                    OrderId = reservation.OrderId,
                    ReservationDate = parsedDate,
                    WarehouseCode = reservation.WarehouseCode,
                    StatusId = 1 // Giả định trạng thái mặc định
                };

                await _reservationRepository.AddAsync(entity, saveChanges: false);

                // Tạo ReservationDetail theo FIFO
                foreach (var orderDetail in orderDetails)
                {
                    double remainingQuantity = orderDetail.Quantity;
                    if (remainingQuantity <= 0)
                        continue;

                    var inventoryItems = await _inventoryRepository.GetInventoryByProductCodeAsync(orderDetail.ProductCode, reservation.WarehouseCode!)
                        .OrderBy(x => x.ReceiveDate)
                        .ToListAsync();

                    foreach (var inventory in inventoryItems)
                    {
                        if (remainingQuantity <= 0)
                            break;

                        // Tính tồn kho khả dụng sau khi trừ reservation hiện có, chỉ tính các reservation chưa hoàn thành (StatusId != 3)
                        var reservedQuantity = await _context.ReservationDetails
                            .Join(_context.Reservations,
                                  rd => rd.ReservationCode,
                                  r => r.ReservationCode,
                                  (rd, r) => new { ReservationDetail = rd, Reservation = r })
                            .Where(x => x.ReservationDetail.ProductCode == inventory.ProductCode &&
                                        x.ReservationDetail.Lotno == inventory.Lotno &&
                                        x.ReservationDetail.LocationCode == inventory.LocationCode &&
                                        x.Reservation.StatusId != 3 &&
                                        x.Reservation.ReservationCode != reservation.ReservationCode) // Loại trừ reservation hiện tại
                            .SumAsync(x => (double?)x.ReservationDetail.QuantityReserved) ?? 0;

                        double availableQuantity = (inventory.Quantity ?? 0) - reservedQuantity;
                        if (availableQuantity <= 0)
                            continue;

                        double quantityToReserve = Math.Min(remainingQuantity, availableQuantity);
                        remainingQuantity -= quantityToReserve;

                        // Kiểm tra ReservationDetail đã tồn tại chưa
                        var existingDetail = await _context.ReservationDetails
                            .FirstOrDefaultAsync(rd => rd.ReservationCode == reservation.ReservationCode &&
                                                       rd.ProductCode == orderDetail.ProductCode &&
                                                       rd.Lotno == inventory.Lotno &&
                                                       rd.LocationCode == inventory.LocationCode);

                        if (existingDetail != null)
                        {
                            existingDetail.QuantityReserved += (float)quantityToReserve;
                            _context.ReservationDetails.Update(existingDetail);
                        }
                        else
                        {
                            var reservationDetail = new ReservationDetail
                            {
                                ReservationCode = reservation.ReservationCode,
                                ProductCode = orderDetail.ProductCode,
                                Lotno = inventory.Lotno,
                                LocationCode = inventory.LocationCode,
                                QuantityReserved = (float)quantityToReserve
                            };
                            await _context.ReservationDetails.AddAsync(reservationDetail);
                        }
                    }

                    if (remainingQuantity > 0)
                    {
                        await transaction.RollbackAsync();
                        return new ServiceResponse<bool>(false, $"Lỗi nội bộ: Không đủ tồn kho dù đã kiểm tra trước. Vui lòng thử lại.");
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Thêm reservation và chi tiết thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi thêm reservation: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi thêm reservation: {ex.Message}");
            }
        }
        public async Task<ServiceResponse<bool>> DeleteReservationAsync(string reservationCode)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var reservation = await _reservationRepository.GetReservationWithCode(reservationCode);
                if (reservation == null)
                    return new ServiceResponse<bool>(false, "Reservation không tồn tại");

                await _reservationRepository.DeleteAsync(reservationCode, saveChanges: false);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Xóa reservation thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi xóa reservation: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi xóa reservation: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> UpdateReservation(ReservationRequestDTO reservation)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            string[] formats = {
                "M/d/yyyy h:mm:ss tt",
                "MM/dd/yyyy hh:mm:ss tt",
                "dd/MM/yyyy"
            };
            if (!DateTime.TryParseExact(reservation.ReservationDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return new ServiceResponse<bool>(false, "Ngày giữ hàng không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }
            try
            {
                var existingReservation = await _reservationRepository.GetReservationWithCode(reservation.ReservationCode);
                if (existingReservation == null)
                    return new ServiceResponse<bool>(false, "Reservation không tồn tại");

                existingReservation.OrderTypeCode = reservation.OrderTypeCode;
                existingReservation.OrderId = reservation.OrderId;
                existingReservation.ReservationDate = parsedDate;
                existingReservation.WarehouseCode = reservation.WarehouseCode;
                existingReservation.StatusId = reservation.StatusId;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Cập nhật reservation thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi cập nhật reservation: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật reservation: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<OrderTypeResponseDTO>>> GetListOrderType(string prefix)
        {
            try
            {
                var orderTypes = await _context.OrderTypes
                    .Where(x => string.IsNullOrEmpty(prefix) || x.OrderTypeCode.Contains(prefix) || x.OrderTypeName!.Contains(prefix))
                    .Select(x => new OrderTypeResponseDTO
                    {
                        OrderTypeCode = x.OrderTypeCode,
                        OrderTypeName = x.OrderTypeName
                    })
                    .ToListAsync();

                return new ServiceResponse<List<OrderTypeResponseDTO>>(true, "Lấy danh sách OrderType thành công", orderTypes);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<OrderTypeResponseDTO>>(false, $"Lỗi khi lấy danh sách OrderType: {ex.Message}");
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

        public async Task<ServiceResponse<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission(string[] warehouseCodes)
        {
            try
            {
                var warehouses = await _context.WarehouseMasters
                    .Where(x => warehouseCodes == null || warehouseCodes.Length == 0 || warehouseCodes.Contains(x.WarehouseCode))
                    .Select(x => new WarehouseMasterResponseDTO
                    {
                        WarehouseCode = x.WarehouseCode,
                        WarehouseName = x.WarehouseName
                    })
                    .ToListAsync();

                return new ServiceResponse<List<WarehouseMasterResponseDTO>>(true, "Lấy danh sách WarehousePermission thành công", warehouses);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<WarehouseMasterResponseDTO>>(false, $"Lỗi khi lấy danh sách WarehousePermission: {ex.Message}");
            }
        }
        public async Task<ServiceResponse<ReservationResponseDTO>> GetReservationsByStockOutCodeAsync(string stockOutCode)
        {
            try
            {
                var reservation = await _context.Reservations
                    .Where(r => r.OrderTypeCode == "SO" || r.OrderTypeCode!.StartsWith("SO"))
                    .Where(r => r.OrderId == stockOutCode)
                    .Select(r => new ReservationResponseDTO
                    {
                        ReservationCode = r.ReservationCode,
                        OrderTypeCode = r.OrderTypeCode,
                        OrderTypeName = r.OrderTypeCodeNavigation!.OrderTypeName,
                        OrderId = r.OrderId,
                        ReservationDate = r.ReservationDate!.Value.ToString("dd/MM/yyyy"),
                        WarehouseCode = r.WarehouseCode,
                        WarehouseName = r.WarehouseCodeNavigation!.WarehouseName,
                        StatusId = r.StatusId,
                        StatusName = r.Status!.StatusName
                    })
                    .FirstOrDefaultAsync();

                if (reservation == null)
                {
                    return new ServiceResponse<ReservationResponseDTO>(false, "Không tìm thấy reservation theo StockOutCode");
                }

                return new ServiceResponse<ReservationResponseDTO>(true, "Lấy reservation theo StockOutCode thành công", reservation);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ReservationResponseDTO>(false, $"Lỗi khi lấy reservation theo StockOutCode: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<ReservationResponseDTO>> GetReservationsByMovementCodeAsync(string movementCode)
        {
            try
            {
                var reservation = await _context.Reservations
                    .Where(r => r.OrderTypeCode == "MO" || r.OrderTypeCode!.StartsWith("MO"))
                    .Where(r => r.OrderId == movementCode)
                    .Select(r => new ReservationResponseDTO
                    {
                        ReservationCode = r.ReservationCode,
                        OrderTypeCode = r.OrderTypeCode,
                        OrderTypeName = r.OrderTypeCodeNavigation!.OrderTypeName,
                        OrderId = r.OrderId,
                        ReservationDate = r.ReservationDate!.Value.ToString("dd/MM/yyyy"),
                        WarehouseCode = r.WarehouseCode,
                        WarehouseName = r.WarehouseCodeNavigation!.WarehouseName,
                        StatusId = r.StatusId,
                        StatusName = r.Status!.StatusName
                    })
                    .FirstOrDefaultAsync();

                if (reservation == null)
                {
                    return new ServiceResponse<ReservationResponseDTO>(false, "Không tìm thấy reservation theo MovementCode");
                }

                return new ServiceResponse<ReservationResponseDTO>(true, "Lấy reservation theo MovementCode thành công", reservation);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ReservationResponseDTO>(false, $"Lỗi khi lấy reservation theo MovementCode: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<ReservationResponseDTO>> GetReservationsByTransferCodeAsync(string transferCode)
        {
            try
            {
                var reservation = await _context.Reservations
                    .Where(r => r.OrderTypeCode == "TF" || r.OrderTypeCode!.StartsWith("TF"))
                    .Where(r => r.OrderId == transferCode)
                    .Select(r => new ReservationResponseDTO
                    {
                        ReservationCode = r.ReservationCode,
                        OrderTypeCode = r.OrderTypeCode,
                        OrderTypeName = r.OrderTypeCodeNavigation!.OrderTypeName,
                        OrderId = r.OrderId,
                        ReservationDate = r.ReservationDate!.Value.ToString("dd/MM/yyyy"),
                        WarehouseCode = r.WarehouseCode,
                        WarehouseName = r.WarehouseCodeNavigation!.WarehouseName,
                        StatusId = r.StatusId,
                        StatusName = r.Status!.StatusName
                    })
                    .FirstOrDefaultAsync();

                if (reservation == null)
                {
                    return new ServiceResponse<ReservationResponseDTO>(false, "Không tìm thấy reservation theo TransferCode");
                }

                return new ServiceResponse<ReservationResponseDTO>(true, "Lấy reservation theo TransferCode thành công", reservation);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ReservationResponseDTO>(false, $"Lỗi khi lấy reservation theo TransferCode: {ex.Message}");
            }
        }
    }
}