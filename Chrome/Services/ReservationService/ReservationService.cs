using Chrome.DTO;
using Chrome.DTO.OrderDetailBaseDTO;
using Chrome.DTO.OrderTypeDTO;
using Chrome.DTO.ReservationDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.DTO.WarehouseMasterDTO;
using Chrome.Models;
using Chrome.Repositories.BOMComponentRepository;
using Chrome.Repositories.InventoryRepository;
using Chrome.Repositories.ReservationRepository;
using Chrome.Services.BOMMasterService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Globalization;

namespace Chrome.Services.ReservationService
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IBOMComponentRepository _bOMComponentRepository;
        private readonly ChromeContext _context;

        public ReservationService(IReservationRepository reservationRepository,IBOMComponentRepository bOMComponentRepository, IInventoryRepository inventoryRepository, ChromeContext context)
        {
            _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
            _inventoryRepository = inventoryRepository;
            _bOMComponentRepository = bOMComponentRepository;
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
        public async Task<ServiceResponse<bool>> AddOrUpdateReservation(ReservationRequestDTO reservation, IDbContextTransaction transaction = null!)
        {
            bool isExternalTransaction = transaction != null;
            transaction ??= await _context.Database.BeginTransactionAsync();

            try
            {
                // Validate date format
                string[] formats = { "M/d/yyyy h:mm:ss tt", "MM/dd/yyyy hh:mm:ss tt", "dd/MM/yyyy" };
                if (!DateTime.TryParseExact(reservation.ReservationDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    return new ServiceResponse<bool>(false, "Ngày giữ hàng không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
                }

                // Get order details based on OrderTypeCode
                var orderDetails = await GetOrderDetailsAsync(reservation.OrderTypeCode!, reservation.OrderId!);
                if (!orderDetails.Any())
                {
                    return new ServiceResponse<bool>(false, reservation.OrderTypeCode!.StartsWith("MO")
                        ? $"Không tìm thấy chi tiết BOM cho mã lệnh sản xuất {reservation.OrderId}."
                        : "Không tìm thấy chi tiết đơn hàng để tạo ReservationDetail.");
                }

                // Check inventory and existing reservations
                bool isMO = reservation.OrderTypeCode!.StartsWith("MO");
                var shortageErrors = await ValidateInventoryAsync(reservation, orderDetails, isMO);
                if (!isMO && shortageErrors.Any())
                {
                    return new ServiceResponse<bool>(false, string.Join("\n", shortageErrors));
                }

                // Update or create reservation
                var existingReservation = await _context.Reservations
                    .FirstOrDefaultAsync(x => x.ReservationCode == reservation.ReservationCode);

                if (existingReservation != null)
                {
                    existingReservation.OrderTypeCode = reservation.OrderTypeCode;
                    existingReservation.OrderId = reservation.OrderId;
                    existingReservation.ReservationDate = parsedDate;
                    existingReservation.WarehouseCode = reservation.WarehouseCode;
                    existingReservation.StatusId = reservation.StatusId;
                    _context.Reservations.Update(existingReservation);
                }
                else
                {
                    var newReservation = new Reservation
                    {
                        ReservationCode = reservation.ReservationCode,
                        OrderTypeCode = reservation.OrderTypeCode,
                        OrderId = reservation.OrderId,
                        ReservationDate = parsedDate,
                        WarehouseCode = reservation.WarehouseCode,
                        StatusId = 1
                    };
                    await _reservationRepository.AddAsync(newReservation, saveChanges: false);
                }

                // Create or update ReservationDetails using FIFO
                await CreateOrUpdateReservationDetailsAsync(reservation, orderDetails);

                await _context.SaveChangesAsync();
                if (!isExternalTransaction) await transaction.CommitAsync();

                return shortageErrors.Any()
                    ? new ServiceResponse<bool>(true, $"Thêm reservation thành công nhưng có lỗi thiếu tồn kho:\n{string.Join("\n", shortageErrors)}")
                    : new ServiceResponse<bool>(true, "Thêm reservation và chi tiết thành công");
            }
            catch (DbUpdateException dbEx)
            {
                if (!isExternalTransaction) await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi thêm reservation: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                if (!isExternalTransaction) await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi thêm reservation: {ex.Message}");
            }
        }

        private async Task<List<OrderDetailBaseDTO>> GetOrderDetailsAsync(string orderTypeCode, string orderId)
        {
            if (orderTypeCode.StartsWith("SO"))
            {
                return await _context.StockOutDetails
                    .Where(od => od.StockOutCode == orderId)
                    .Select(od => new OrderDetailBaseDTO
                    {
                        ProductCode = od.ProductCode,
                        Quantity = od.Demand ?? 0
                    })
                    .ToListAsync();
            }
            else if (orderTypeCode.StartsWith("MV"))
            {
                return await _context.MovementDetails
                    .Where(od => od.MovementCode == orderId)
                    .Select(od => new OrderDetailBaseDTO
                    {
                        ProductCode = od.ProductCode,
                        Quantity = od.Demand ?? 0
                    })
                    .ToListAsync();
            }
            else if (orderTypeCode.StartsWith("TF"))
            {
                return await _context.TransferDetails
                    .Where(od => od.TransferCode == orderId)
                    .Select(od => new OrderDetailBaseDTO
                    {
                        ProductCode = od.ProductCode,
                        Quantity = od.Demand ?? 0
                    })
                    .ToListAsync();
            }
            else if (orderTypeCode.StartsWith("MO"))
            {
                return await _context.ManufacturingOrderDetails
                   .Where(od => od.ManufacturingOrderCode == orderId)
                   .Select(od => new OrderDetailBaseDTO
                   {
                       ProductCode = od.ComponentCode,
                       Quantity = od.ToConsumeQuantity ?? 0
                   })
                   .ToListAsync();
            }

            return new List<OrderDetailBaseDTO>();
        }

        private async Task<List<string>> ValidateInventoryAsync(ReservationRequestDTO reservation, List<OrderDetailBaseDTO> orderDetails, bool isMO)
        {
            var shortageErrors = new List<string>();
            foreach (var orderDetail in orderDetails)
            {
                if (orderDetail.Quantity <= 0) continue;

                var inventoryItems = await _inventoryRepository.GetInventoryByProductCodeAsync(orderDetail.ProductCode!, reservation.WarehouseCode!)
                    .OrderBy(x => x.ReceiveDate)
                    .ToListAsync();

                if (!inventoryItems.Any())
                {
                    if (isMO)
                        shortageErrors.Add($"Không có tồn kho cho sản phẩm {orderDetail.ProductCode}");
                    else
                        return new List<string> { $"Không có tồn kho cho sản phẩm {orderDetail.ProductCode}" };
                    continue;
                }

                double remainingQuantity = orderDetail.Quantity;
                foreach (var inventory in inventoryItems)
                {
                    var reservedQuantity = await _context.ReservationDetails
                        .Include(rd => rd.ReservationCodeNavigation)
                        .Where(rd => rd.ProductCode == inventory.ProductCode &&
                                    rd.Lotno == inventory.Lotno &&
                                    rd.ReservationCodeNavigation!.StatusId != 3)
                        .SumAsync(rd => (double?)rd.QuantityReserved) ?? 0;

                    double availableQuantity = (inventory.Quantity ?? 0) - reservedQuantity;
                    if (availableQuantity > 0)
                    {
                        remainingQuantity -= availableQuantity;
                        if (remainingQuantity <= 0) break;
                    }
                }

                if (remainingQuantity > 0)
                {
                    var errorMessage = $"Thiếu {remainingQuantity} đơn vị tồn kho cho sản phẩm {orderDetail.ProductCode} sau khi trừ reservation";
                    if (isMO)
                        shortageErrors.Add(errorMessage);
                    else
                        return new List<string> { errorMessage };
                }
            }
            return shortageErrors;
        }

        private async Task CreateOrUpdateReservationDetailsAsync(ReservationRequestDTO reservation, List<OrderDetailBaseDTO> orderDetails)
        {
            foreach (var orderDetail in orderDetails)
            {
                double remainingQuantity = orderDetail.Quantity;
                if (remainingQuantity <= 0) continue;

                var inventoryItems = await _inventoryRepository.GetInventoryByProductCodeAsync(orderDetail.ProductCode!, reservation.WarehouseCode!)
                    .OrderBy(x => x.ReceiveDate)
                    .ToListAsync();

                foreach (var inventory in inventoryItems)
                {
                    if (remainingQuantity <= 0) break;

                    var reservedQuantity = await _context.ReservationDetails
                        .Join(_context.Reservations,
                              rd => rd.ReservationCode,
                              r => r.ReservationCode,
                              (rd, r) => new { ReservationDetail = rd, Reservation = r })
                        .Where(x => x.ReservationDetail.ProductCode == inventory.ProductCode &&
                                    x.ReservationDetail.Lotno == inventory.Lotno &&
                                    x.ReservationDetail.LocationCode == inventory.LocationCode &&
                                    x.Reservation.StatusId != 3 &&
                                    x.Reservation.ReservationCode != reservation.ReservationCode)
                        .SumAsync(x => (double?)x.ReservationDetail.QuantityReserved) ?? 0;

                    double availableQuantity = (inventory.Quantity ?? 0) - reservedQuantity;
                    if (availableQuantity <= 0) continue;

                    double quantityToReserve = Math.Min(remainingQuantity, availableQuantity);
                    remainingQuantity -= quantityToReserve;

                    var existingDetail = await _context.ReservationDetails
                        .FirstOrDefaultAsync(rd => rd.ReservationCode == reservation.ReservationCode &&
                                                  rd.ProductCode == orderDetail.ProductCode &&
                                                  rd.Lotno == inventory.Lotno &&
                                                  rd.LocationCode == inventory.LocationCode);

                    if (existingDetail != null)
                    {
                        existingDetail.QuantityReserved = (float)quantityToReserve;
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

        public async Task<ServiceResponse<ReservationResponseDTO>> GetReservationsByManufacturingCodeAsync(string manufacturingCode)
        {
            try
            {
                var reservation = await _context.Reservations
                    .Where(r => r.OrderTypeCode == "MO" || r.OrderTypeCode!.StartsWith("MO"))
                    .Where(r => r.OrderId == manufacturingCode)
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
                    return new ServiceResponse<ReservationResponseDTO>(false, "Không tìm thấy reservation theo lệnh sản xuất");
                }

                return new ServiceResponse<ReservationResponseDTO>(true, "Lấy reservation theo lệnh sản xuất thành công", reservation);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ReservationResponseDTO>(false, $"Lỗi khi lấy reservation theo lệnh sản xuất: {ex.Message}");
            }
        }
    }
}