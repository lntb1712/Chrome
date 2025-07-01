using Chrome.DTO;
using Chrome.DTO.MovementDetailDTO;
using Chrome.DTO.PickListDTO;
using Chrome.DTO.ProductMasterDTO;
using Chrome.DTO.PutAwayDTO;
using Chrome.DTO.ReservationDTO;
using Chrome.Models;
using Chrome.Repositories.InventoryRepository;
using Chrome.Repositories.MovementDetailRepository;
using Chrome.Repositories.PickListDetailRepository;
using Chrome.Repositories.ProductMasterRepository;
using Chrome.Services.PickListService;
using Chrome.Services.PutAwayService;
using Chrome.Services.ReservationService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Chrome.Services.MovementDetailService
{
    public class MovementDetailService : IMovementDetailService
    {
        private readonly IMovementDetailRepository _movementDetailRepository;
        private readonly IProductMasterRepository _productMasterRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IReservationService _reservationService;
        private readonly IPickListService _pickListService;
        private readonly IPutAwayService _putAwayService;
        private readonly IPickListDetailRepository _pickListDetailRepository;
        private readonly ChromeContext _context;

        public MovementDetailService(
            IMovementDetailRepository movementDetailRepository,
            IProductMasterRepository productMasterRepository,
            IInventoryRepository inventoryRepository,
            IPickListDetailRepository pickListDetailRepository,
            IReservationService reservationService,
            IPickListService pickListService,
            IPutAwayService putAwayService,
            ChromeContext context)
        {
            _movementDetailRepository = movementDetailRepository ?? throw new ArgumentNullException(nameof(movementDetailRepository));
            _productMasterRepository = productMasterRepository ?? throw new ArgumentNullException(nameof(productMasterRepository));
            _inventoryRepository = inventoryRepository ?? throw new ArgumentNullException(nameof(inventoryRepository));
            _pickListDetailRepository = pickListDetailRepository ?? throw new ArgumentNullException(nameof(pickListDetailRepository));
            _reservationService = reservationService ?? throw new ArgumentNullException(nameof(reservationService));
            _pickListService = pickListService ?? throw new ArgumentNullException(nameof(pickListService));
            _putAwayService = putAwayService ?? throw new ArgumentNullException(nameof(putAwayService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ServiceResponse<PagedResponse<MovementDetailResponseDTO>>> GetAllMovementDetailsAsync(string[] warehouseCodes, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _movementDetailRepository.GetAllMovementDetailsAsync(warehouseCodes);
                var totalItems = await query.CountAsync();
                var movementDetails = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(md => new MovementDetailResponseDTO
                    {
                        MovementCode = md.MovementCode,
                        ProductCode = md.ProductCode,
                        ProductName = md.ProductCodeNavigation!.ProductName!,
                        Demand = md.Demand,
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<MovementDetailResponseDTO>(movementDetails, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<MovementDetailResponseDTO>>(true, "Lấy danh sách chi tiết movement thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<MovementDetailResponseDTO>>(false, $"Lỗi khi lấy danh sách chi tiết movement: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<MovementDetailResponseDTO>>> GetMovementDetailsByMovementCodeAsync(string movementCode, int page = 1, int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(movementCode))
                {
                    return new ServiceResponse<PagedResponse<MovementDetailResponseDTO>>(false, "Mã movement không được để trống");
                }

                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _movementDetailRepository.GetMovementDetailsByMovementCodeAsync(movementCode);
                var totalItems = await query.CountAsync();
                var movementDetails = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(md => new MovementDetailResponseDTO
                    {
                        MovementCode = md.MovementCode,
                        ProductCode = md.ProductCode,
                        ProductName = md.ProductCodeNavigation!.ProductName!,
                        Demand = md.Demand,
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<MovementDetailResponseDTO>(movementDetails, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<MovementDetailResponseDTO>>(true, "Lấy chi tiết movement theo mã thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<MovementDetailResponseDTO>>(false, $"Lỗi khi lấy chi tiết movement theo mã: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<MovementDetailResponseDTO>>> SearchMovementDetailsAsync(string[] warehouseCodes, string movementCode, string textToSearch, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _movementDetailRepository.SearchMovementDetailsAsync(warehouseCodes, movementCode, textToSearch);
                var totalItems = await query.CountAsync();
                var movementDetails = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(md => new MovementDetailResponseDTO
                    {
                        MovementCode = md.MovementCode,
                        ProductCode = md.ProductCode,
                        ProductName = md.ProductCodeNavigation!.ProductName!,
                        Demand = md.Demand,
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<MovementDetailResponseDTO>(movementDetails, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<MovementDetailResponseDTO>>(true, "Tìm kiếm chi tiết movement thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<MovementDetailResponseDTO>>(false, $"Lỗi khi tìm kiếm chi tiết movement: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> AddMovementDetail(MovementDetailRequestDTO movementDetail)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validate input
                if (movementDetail == null || string.IsNullOrEmpty(movementDetail.MovementCode) || string.IsNullOrEmpty(movementDetail.ProductCode))
                {
                    return new ServiceResponse<bool>(false, "Dữ liệu đầu vào không hợp lệ. MovementCode và ProductCode là bắt buộc.");
                }

                // Check if movement detail already exists
                var existingDetail = await _context.MovementDetails
                    .FirstOrDefaultAsync(md => md.MovementCode == movementDetail.MovementCode && md.ProductCode == movementDetail.ProductCode);
                if (existingDetail != null)
                {
                    return new ServiceResponse<bool>(false, "Chi tiết movement đã tồn tại.");
                }

                // Validate demand
                if (movementDetail.Demand <= 0)
                {
                    return new ServiceResponse<bool>(false, "Số lượng yêu cầu (Demand) phải lớn hơn 0.");
                }

                // Get Movement info
                var movement = await _context.Movements
                    .FirstOrDefaultAsync(m => m.MovementCode == movementDetail.MovementCode);
                if (movement == null)
                {
                    return new ServiceResponse<bool>(false, $"Không tìm thấy Movement với mã {movementDetail.MovementCode}.");
                }

                var warehouseCode = movement.WarehouseCode;
                var fromLocation = movement.FromLocation;
                var toLocation = movement.ToLocation;
                if (string.IsNullOrWhiteSpace(warehouseCode) || string.IsNullOrWhiteSpace(fromLocation) || string.IsNullOrWhiteSpace(toLocation))
                {
                    return new ServiceResponse<bool>(false, "Movement không có WarehouseCode, FromLocation hoặc ToLocation hợp lệ.");
                }

                // Check inventory at FromLocation
                var inventoryQuery =  _inventoryRepository.GetInventoryByProductCodeAsync(movementDetail.ProductCode, warehouseCode);
                var totalOnHand = await inventoryQuery
                    .Where(i => i.LocationCode == fromLocation)
                    .SumAsync(i => i.Quantity ?? 0);
                if (movementDetail.Demand > totalOnHand)
                {
                    return new ServiceResponse<bool>(false, $"Số lượng yêu cầu ({movementDetail.Demand}) vượt quá tồn kho hiện có ({totalOnHand}) cho sản phẩm {movementDetail.ProductCode} tại vị trí {fromLocation}.");
                }

                // Create MovementDetail
                var newMovementDetail = new MovementDetail
                {
                    MovementCode = movementDetail.MovementCode,
                    ProductCode = movementDetail.ProductCode,
                    Demand = movementDetail.Demand,
                };
                _context.MovementDetails.Add(newMovementDetail);
                await _context.SaveChangesAsync(); // Lưu để sử dụng trong Reservation

                // Create Reservation
                var reservationCode = $"RES_{movement.MovementCode}";
                var reservationRequest = new ReservationRequestDTO
                {
                    ReservationCode = reservationCode,
                    OrderTypeCode = movement.OrderTypeCode, // Movement
                    OrderId = movementDetail.MovementCode,
                    ReservationDate = DateTime.Now.ToString("dd/MM/yyyy"),
                    WarehouseCode = warehouseCode
                };
                var reservationResponse = await _reservationService.AddOrUpdateReservation(reservationRequest,transaction);
                if (!reservationResponse.Success)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi tạo reservation: {reservationResponse.Message}");
                }

                // Create Picklist
                var pickListCode = $"PICK_{movement.MovementCode}";
                var pickListRequest = new PickListRequestDTO
                {
                    PickNo = pickListCode,
                    ReservationCode = reservationCode,
                    WarehouseCode = warehouseCode,
                    Responsible = movement.Responsible,
                    PickDate = DateTime.Now.ToString("dd/MM/yyyy"),
                    StatusId = 1
                };
                var pickListResponse = await _pickListService.AddPickList(pickListRequest,transaction);
                if (!pickListResponse.Success)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false,  $"Lỗi khi tạo picklist: {pickListResponse.Message}");
                }

                // Create Putaway
                var putAwayCode = $"PUT_{movement.MovementCode}";
                var putAwayRequest = new PutAwayRequestDTO
                {
                    PutAwayCode = putAwayCode,
                    OrderTypeCode = movement.OrderTypeCode,
                    LocationCode = toLocation, // ToLocation
                    Responsible = movement.Responsible,
                    StatusId = 1,
                    PutAwayDate = DateTime.Now.ToString("dd/MM/yyyy"),
                    PutAwayDescription = $"Cất hàng cho lệnh chuyển kệ {movement.MovementCode}"
                };
                var putAwayResponse = await _putAwayService.AddPutAway(putAwayRequest, transaction);
                if (!putAwayResponse.Success)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi tạo putaway: {putAwayResponse.Message}");
                }

                var pickListDetail = _pickListDetailRepository.GetPickListDetailsByPickNoAsync(pickListCode);
                foreach (var pickDetail in pickListDetail)
                {
                    var putAwayDetail = new PutAwayDetail
                    {
                        PutAwayCode = putAwayCode,
                        ProductCode = pickDetail.ProductCode!,
                        LotNo = pickDetail.LotNo!,
                        Demand = pickDetail.Demand,
                        Quantity = 0,
                    };
                    await _context.PutAwayDetails.AddAsync(putAwayDetail);
                    
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Thêm chi tiết movement, reservation, picklist và putaway thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi thêm chi tiết movement: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi thêm chi tiết movement: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> UpdateMovementDetail(MovementDetailRequestDTO movementDetail)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (movementDetail == null || string.IsNullOrEmpty(movementDetail.MovementCode) || string.IsNullOrEmpty(movementDetail.ProductCode))
                {
                    return new ServiceResponse<bool>(false, "Dữ liệu đầu vào không hợp lệ. MovementCode và ProductCode là bắt buộc.");
                }

                var existingDetail = await _context.MovementDetails
                    .FirstOrDefaultAsync(md => md.MovementCode == movementDetail.MovementCode && md.ProductCode == movementDetail.ProductCode);

                if (existingDetail == null)
                {
                    return new ServiceResponse<bool>(false, "Chi tiết movement không tồn tại.");
                }

                // Update allowed fields
                existingDetail.Demand = movementDetail.Demand ?? existingDetail.Demand;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Cập nhật chi tiết movement thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi cập nhật chi tiết movement: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật chi tiết movement: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteMovementDetail(string movementCode, string productCode)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(movementCode) || string.IsNullOrEmpty(productCode))
                {
                    return new ServiceResponse<bool>(false, "MovementCode và ProductCode không được để trống.");
                }

                var movementDetail = await _context.MovementDetails
                    .FirstOrDefaultAsync(md => md.MovementCode == movementCode && md.ProductCode == productCode);

                if (movementDetail == null)
                {
                    return new ServiceResponse<bool>(false, "Chi tiết movement không tồn tại.");
                }

                Expression<Func<MovementDetail, bool>> predicate = x => x.MovementCode == movementCode && x.ProductCode == productCode;
                await _movementDetailRepository.DeleteFirstByConditionAsync(predicate);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Xóa chi tiết movement thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi xóa chi tiết movement: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi xóa chi tiết movement: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<ProductMasterResponseDTO>> GetProductByLocationCode(string locationCode)
        {
            try
            {
                if (string.IsNullOrEmpty(locationCode))
                {
                    return new ServiceResponse<ProductMasterResponseDTO>(false, "LocationCode không được để trống.");
                }

                var product = await _productMasterRepository.GetProductWithLocationCode(locationCode);
                if (product == null)
                {
                    return new ServiceResponse<ProductMasterResponseDTO>(false, "Không tìm thấy sản phẩm với LocationCode được cung cấp.");
                }

                var response = new ProductMasterResponseDTO
                {
                    ProductCode = product.ProductCode,
                    ProductName = product.ProductName,
                    ProductDescription = product.ProductDescription,
                    ProductImage = product.ProductImage,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category?.CategoryName ?? "Không có danh mục",
                    BaseQuantity = product.BaseQuantity,
                    Uom = product.Uom,
                    BaseUom = product.BaseUom,
                    Valuation = (float?)product.Valuation,
                    TotalOnHand = (float)(product.Inventories?.Where(t => t.ProductCode == product.ProductCode).Sum(i => i.Quantity) ?? 0.00)
                };

                return new ServiceResponse<ProductMasterResponseDTO>(true, "Lấy sản phẩm thành công", response);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ProductMasterResponseDTO>(false, $"Lỗi khi lấy sản phẩm: {ex.Message}");
            }
        }
    }
}