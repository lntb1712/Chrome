using Chrome.DTO;
using Chrome.DTO.InventoryDTO;
using Chrome.DTO.PickListDTO;
using Chrome.DTO.ProductMasterDTO;
using Chrome.DTO.PutAwayDTO;
using Chrome.DTO.ReservationDTO;
using Chrome.DTO.TransferDetailDTO;
using Chrome.Models;
using Chrome.Repositories.InventoryRepository;
using Chrome.Repositories.PickListDetailRepository;
using Chrome.Repositories.ProductMasterRepository;
using Chrome.Repositories.PutAwayRulesRepository;
using Chrome.Repositories.TransferDetailRepository;
using Chrome.Services.PickListService;
using Chrome.Services.PutAwayService;
using Chrome.Services.ReservationService;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Chrome.Services.TransferDetailService
{
    public class TransferDetailService : ITransferDetailService
    {
        private readonly ITransferDetailRepository _transferDetailRepository;
        private readonly IProductMasterRepository _productMasterRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IReservationService _reservationService;
        private readonly IPickListService _pickListService;
        private readonly IPutAwayService _putAwayService;
        private readonly IPutAwayRulesRepository _putAwayRulesRepository;
        private readonly IPickListDetailRepository _pickListDetailRepository;
        private readonly ChromeContext _context;

        public TransferDetailService(
            ITransferDetailRepository transferDetailRepository,
            IProductMasterRepository productMasterRepository,
            IInventoryRepository inventoryRepository,
            IReservationService reservationService,
            IPickListService pickListService,
            IPutAwayService putAwayService,
            IPutAwayRulesRepository putAwayRulesRepository,
            IPickListDetailRepository pickListDetailRepository,
            ChromeContext context)
        {
            _transferDetailRepository = transferDetailRepository ?? throw new ArgumentNullException(nameof(transferDetailRepository));
            _productMasterRepository = productMasterRepository ?? throw new ArgumentNullException(nameof(productMasterRepository));
            _inventoryRepository = inventoryRepository ?? throw new ArgumentNullException(nameof(inventoryRepository));
            _reservationService = reservationService ?? throw new ArgumentNullException(nameof(reservationService));
            _pickListService = pickListService ?? throw new ArgumentNullException(nameof(pickListService));
            _putAwayService = putAwayService;
            _putAwayRulesRepository = putAwayRulesRepository;
            _pickListDetailRepository = pickListDetailRepository;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ServiceResponse<PagedResponse<TransferDetailResponseDTO>>> GetAllTransferDetailsAsync(string[] warehouseCodes, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _transferDetailRepository.GetAllTransferDetailsAsync(warehouseCodes);
                var totalItems = await query.CountAsync();
                var transferDetails = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(td => new TransferDetailResponseDTO
                    {
                        TransferCode = td.TransferCode,
                        ProductCode = td.ProductCode,
                        ProductName = td.ProductCodeNavigation!.ProductName!,
                        Demand = td.Demand,
                        QuantityInBounded = td.QuantityInBounded,
                        QuantityOutBounded = td.QuantityOutBounded,
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<TransferDetailResponseDTO>(transferDetails, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<TransferDetailResponseDTO>>(true, "Lấy danh sách chi tiết chuyển kho thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<TransferDetailResponseDTO>>(false, $"Lỗi khi lấy danh sách chi tiết chuyển kho: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<TransferDetailResponseDTO>>> GetTransferDetailsByTransferCodeAsync(string transferCode, int page = 1, int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(transferCode))
                {
                    return new ServiceResponse<PagedResponse<TransferDetailResponseDTO>>(false, "Mã chuyển kho không được để trống");
                }

                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _transferDetailRepository.GetTransferDetailsByTransferCodeAsync(transferCode);
                var totalItems = await query.CountAsync();
                var transferDetails = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(td => new TransferDetailResponseDTO
                    {
                        TransferCode = td.TransferCode,
                        ProductCode = td.ProductCode,
                        ProductName = td.ProductCodeNavigation!.ProductName!,
                        Demand = td.Demand,
                        QuantityInBounded = td.QuantityInBounded,
                        QuantityOutBounded = td.QuantityOutBounded,
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<TransferDetailResponseDTO>(transferDetails, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<TransferDetailResponseDTO>>(true, "Lấy chi tiết chuyển kho theo mã thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<TransferDetailResponseDTO>>(false, $"Lỗi khi lấy chi tiết chuyển kho theo mã: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<TransferDetailResponseDTO>>> SearchTransferDetailsAsync(string[] warehouseCodes, string transferCode, string textToSearch, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _transferDetailRepository.SearchTransferDetailsAsync(warehouseCodes, transferCode, textToSearch);
                var totalItems = await query.CountAsync();
                var transferDetails = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(td => new TransferDetailResponseDTO
                    {
                        TransferCode = td.TransferCode,
                        ProductCode = td.ProductCode,
                        ProductName = td.ProductCodeNavigation!.ProductName!,
                        Demand = td.Demand,
                        QuantityInBounded = td.QuantityInBounded,
                        QuantityOutBounded = td.QuantityOutBounded,
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<TransferDetailResponseDTO>(transferDetails, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<TransferDetailResponseDTO>>(true, "Tìm kiếm chi tiết chuyển kho thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<TransferDetailResponseDTO>>(false, $"Lỗi khi tìm kiếm chi tiết chuyển kho: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> AddTransferDetail(TransferDetailRequestDTO transferDetail)
        {
                using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validate input
                if (transferDetail == null || string.IsNullOrEmpty(transferDetail.TransferCode) || string.IsNullOrEmpty(transferDetail.ProductCode))
                {
                    return new ServiceResponse<bool>(false, "Dữ liệu đầu vào không hợp lệ. TransferCode và ProductCode là bắt buộc.");
                }

                // Check if transfer detail already exists
                var existingDetail = await _context.TransferDetails
                    .FirstOrDefaultAsync(td => td.TransferCode == transferDetail.TransferCode && td.ProductCode == transferDetail.ProductCode);
                if (existingDetail != null)
                {
                    return new ServiceResponse<bool>(false, "Chi tiết chuyển kho đã tồn tại.");
                }

                // Validate demand
                if (transferDetail.Demand <= 0)
                {
                    return new ServiceResponse<bool>(false, "Số lượng yêu cầu (Demand) phải lớn hơn 0.");
                }

                // Get Transfer info
                var transfer = await _context.Transfers
                    .FirstOrDefaultAsync(t => t.TransferCode == transferDetail.TransferCode);
                if (transfer == null)
                {
                    return new ServiceResponse<bool>(false, $"Không tìm thấy Transfer với mã {transferDetail.TransferCode}.");
                }

                var fromWarehouseCode = transfer.FromWarehouseCode;
                var toWarehouseCode = transfer.ToWarehouseCode;
                if (string.IsNullOrWhiteSpace(fromWarehouseCode))
                {
                    return new ServiceResponse<bool>(false, "Transfer không có FromWarehouseCode hợp lệ.");
                }

                // Check inventory at FromWarehouseCode
                var inventoryQuery = _inventoryRepository.GetInventoryByProductCodeAsync(transferDetail.ProductCode, fromWarehouseCode);
                var totalOnHand = await inventoryQuery
                    .SumAsync(i => i.Quantity ?? 0);
                if (transferDetail.Demand > totalOnHand)
                {
                    return new ServiceResponse<bool>(false, $"Số lượng yêu cầu ({transferDetail.Demand}) vượt quá tồn kho hiện có ({totalOnHand}) cho sản phẩm {transferDetail.ProductCode} tại kho {fromWarehouseCode}.");
                }

                // Create TransferDetail
                var newTransferDetail = new TransferDetail
                {
                    TransferCode = transferDetail.TransferCode,
                    ProductCode = transferDetail.ProductCode,
                    Demand = transferDetail.Demand,
                    QuantityInBounded = transferDetail.QuantityInBounded,
                    QuantityOutBounded = transferDetail.QuantityOutBounded,
                };
                _context.TransferDetails.Add(newTransferDetail);
                await _context.SaveChangesAsync(); // Save for Reservation

                // Create Reservation
                var reservationCode = $"RES_{transfer.TransferCode}_{transferDetail.ProductCode}";
                var reservationRequest = new ReservationRequestDTO
                {
                    ReservationCode = reservationCode,
                    OrderTypeCode = transfer.OrderTypeCode,
                    OrderId = transferDetail.TransferCode,
                    ReservationDate = DateTime.Now.ToString("dd/MM/yyyy"),
                    WarehouseCode = fromWarehouseCode
                };
                var reservationResponse = await _reservationService.AddOrUpdateReservation(reservationRequest, transaction);
                if (!reservationResponse.Success)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi tạo reservation: {reservationResponse.Message}");
                }

                // Create Picklist
                var pickListCode = $"PICK_{transfer.TransferCode}_{transferDetail.ProductCode}";
                var pickListRequest = new PickListRequestDTO
                {
                    PickNo = pickListCode,
                    ReservationCode = reservationCode,
                    WarehouseCode = fromWarehouseCode,
                    PickDate = DateTime.Now.ToString("dd/MM/yyyy"),
                    StatusId = 1
                };
                var pickListResponse = await _pickListService.AddPickList(pickListRequest, transaction);
                if (!pickListResponse.Success)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi tạo picklist: {pickListResponse.Message}");
                }
                // Preload PutAwayRules
                var putAwayRulesPaged = await _putAwayRulesRepository.GetAllPutAwayRules(1, int.MaxValue);
                // Handle multiple rules for the same ProductCode by selecting all applicable rules
                var putAwayRulesList = putAwayRulesPaged
                    .Where(x => x.ProductCode == transferDetail.ProductCode && x.LocationCodeNavigation!.WarehouseCode == toWarehouseCode)
                    .ToList();

                // Load ProductMasters and LocationMasters
                var productMasters = (await _productMasterRepository.GetAllProduct(1, int.MaxValue))
                    .GroupBy(p => p.ProductCode!)
                    .ToDictionary(g => g.Key, g => g.First()); // Take the first ProductMaster if duplicates exist
                var locationMasters = await _context.LocationMasters
                    .Where(l => l.WarehouseCode == toWarehouseCode)
                    .ToDictionaryAsync(l => l.LocationCode!, l => l);
                var storageProducts = await _context.StorageProducts
                    .ToDictionaryAsync(sp => sp.StorageProductId!, sp => sp);

                string locationCode;
                if (putAwayRulesList.Any())
                {
                    var checkQuantityForLocation = _inventoryRepository.GetInventoryByProductCodeAsync(transferDetail.ProductCode, transfer.ToWarehouseCode!);
                    var inventoryQuantities = await checkQuantityForLocation
                        .GroupBy(i => i.LocationCode)
                        .Select(g => new { LocationCode = g.Key, TotalQuantity = g.Sum(i => i.Quantity ?? 0) })
                        .ToListAsync();

                    var rule = putAwayRulesList
                        .OrderBy(r => inventoryQuantities.FirstOrDefault(q => q.LocationCode == r.LocationCode)?.TotalQuantity ?? int.MaxValue)
                        .First();
                    locationCode = rule.LocationCode!;
                }
                else
                {
                    locationCode = $"{toWarehouseCode}/VIRTUAL_LOC/{transferDetail.ProductCode}";
                    // StorageProduct
                    var storageProductId = $"SP_{transferDetail.ProductCode}";
                    if (!storageProducts.TryGetValue(storageProductId, out var storageProduct))
                    {
                        // Get BaseQuantity from ProductMaster to calculate MaxQuantity
                        var baseQuantity = productMasters.ContainsKey(transferDetail.ProductCode) ? (productMasters[transferDetail.ProductCode].BaseQuantity ?? 1) : 1;
                        var maxQuantityInBaseUOM = transferDetail.Demand * baseQuantity;

                        storageProduct = new StorageProduct
                        {
                            StorageProductId = storageProductId,
                            StorageProductName = $"Định mức ảo cho {transferDetail.ProductCode}",
                            ProductCode = transferDetail.ProductCode,
                            MaxQuantity = maxQuantityInBaseUOM // Store in base UOM
                        };
                        _context.StorageProducts.Add(storageProduct);
                        storageProducts.Add(storageProductId, storageProduct);
                    }
                    else
                    {
                        // Update MaxQuantity
                        var baseQuantity = productMasters.ContainsKey(transferDetail.ProductCode) ? (productMasters[transferDetail.ProductCode].BaseQuantity ?? 1) : 1;
                        storageProduct.MaxQuantity += transferDetail.Demand * baseQuantity;
                        _context.StorageProducts.Update(storageProduct);
                    }

                    // Virtual Location
                    if (!locationMasters.ContainsKey(locationCode))
                    {
                        var newLocation = new LocationMaster
                        {
                            LocationCode = locationCode,
                            LocationName = $"Vùng ảo cho {transferDetail.ProductCode}",
                            WarehouseCode = toWarehouseCode,
                            StorageProductId = storageProductId
                        };
                        _context.LocationMasters.Add(newLocation);
                        locationMasters.Add(locationCode, newLocation);
                        await _context.SaveChangesAsync(); // lưu location
                    }
                }

                // Create Putaway
                var putAwayCode = $"PUT_{transfer.TransferCode}_{transferDetail.ProductCode}";
                var putAwayRequest = new PutAwayRequestDTO
                {
                    PutAwayCode = putAwayCode,
                    OrderTypeCode = transfer.OrderTypeCode,
                    LocationCode = locationCode,
                    Responsible = transfer.ToResponsible,
                    StatusId = 1,
                    PutAwayDate = DateTime.Now.ToString("dd/MM/yyyy"),
                    PutAwayDescription = $"Cất hàng cho lệnh chuyển kho {transfer.TransferCode}"
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
                return new ServiceResponse<bool>(true, "Thêm chi tiết chuyển kho, reservation, picklist và putaway thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi thêm chi tiết chuyển kho: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi thêm chi tiết chuyển kho: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> UpdateTransferDetail(TransferDetailRequestDTO transferDetail)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (transferDetail == null || string.IsNullOrEmpty(transferDetail.TransferCode) || string.IsNullOrEmpty(transferDetail.ProductCode))
                {
                    return new ServiceResponse<bool>(false, "Dữ liệu đầu vào không hợp lệ. TransferCode và ProductCode là bắt buộc.");
                }

                var existingDetail = await _context.TransferDetails
                    .FirstOrDefaultAsync(td => td.TransferCode == transferDetail.TransferCode && td.ProductCode == transferDetail.ProductCode);

                if (existingDetail == null)
                {
                    return new ServiceResponse<bool>(false, "Chi tiết chuyển kho không tồn tại.");
                }

                // Update allowed fields
                existingDetail.Demand = transferDetail.Demand;
                existingDetail.QuantityOutBounded = transferDetail.QuantityOutBounded;
                existingDetail.QuantityInBounded = transferDetail.QuantityInBounded;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Cập nhật chi tiết chuyển kho thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi cập nhật chi tiết chuyển kho: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật chi tiết chuyển kho: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteTransferDetail(string transferCode, string productCode)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(transferCode) || string.IsNullOrEmpty(productCode))
                {
                    return new ServiceResponse<bool>(false, "TransferCode và ProductCode không được để trống.");
                }

                var transferDetail = await _context.TransferDetails
                    .FirstOrDefaultAsync(td => td.TransferCode == transferCode && td.ProductCode == productCode);

                if (transferDetail == null)
                {
                    return new ServiceResponse<bool>(false, "Chi tiết chuyển kho không tồn tại.");
                }

                Expression<Func<TransferDetail, bool>> predicate = x => x.TransferCode == transferCode && x.ProductCode == productCode;
                await _transferDetailRepository.DeleteFirstByConditionAsync(predicate);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Xóa chi tiết chuyển kho thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi xóa chi tiết chuyển kho: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi xóa chi tiết chuyển kho: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<InventorySummaryDTO>>> GetProductByWarehouseCode(string warehouseCode)
        {
            try
            {
                if (string.IsNullOrEmpty(warehouseCode))
                {
                    return new ServiceResponse<List<InventorySummaryDTO>>(false, "WarehouseCode không được để trống.");
                }

                var query = _inventoryRepository.GetInventoryByWarehouseCodeAsync(warehouseCode);
                var product = await query.GroupBy(x => x.ProductCode)
                .Select(g => new InventorySummaryDTO
                {
                    ProductCode = g.Key,
                    ProductName = g.First().ProductCodeNavigation.ProductName!,
                    CategoryId = g.First().ProductCodeNavigation.CategoryId!,
                    CategoryName = g.First().ProductCodeNavigation.Category!.CategoryName!,
                    Quantity = g.Sum(x => x.Quantity),
                    BaseQuantity = g.Sum(x => x.Quantity / (x.ProductCodeNavigation.BaseQuantity)),
                    UOM = g.First().ProductCodeNavigation.Uom!,
                    BaseUOM = g.First().ProductCodeNavigation.BaseUom!
                })
                .OrderBy(x => x.ProductCode)
                .ToListAsync();

                return new ServiceResponse<List<InventorySummaryDTO>>(true, "Lấy sản phẩm thành công", product);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<InventorySummaryDTO>>(false, $"Lỗi khi lấy sản phẩm: {ex.Message}");
            }
        }
    }
}
