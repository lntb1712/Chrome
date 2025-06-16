using Chrome.DTO;
using Chrome.DTO.InventoryDTO;
using Chrome.DTO.LocationMasterDTO;
using Chrome.DTO.ProductMasterDTO;
using Chrome.DTO.StockInDetailDTO;
using Chrome.DTO.StockInDTO;
using Chrome.DTO.StorageProductDTO;
using Chrome.Models;
using Chrome.Repositories.InventoryRepository;
using Chrome.Repositories.ProductMasterRepository;
using Chrome.Repositories.PutAwayRulesRepository;
using Chrome.Repositories.StockInDetailRepository;
using Chrome.Repositories.StockInRepository;
using Chrome.Repositories.StorageProductRepository;
using Chrome.Services.InventoryService;
using Chrome.Services.LocationMasterService;
using Chrome.Services.StockInService;
using Chrome.Services.StorageProductService;
using DocumentFormat.OpenXml.Office.Word;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;

namespace Chrome.Services.StockInDetailService
{
    public class StockInDetailService : IStockInDetailService
    {
        private readonly IStockInDetailRepository _stockInDetailRepository;
        private readonly IInventoryService _inventoryService;
        private readonly IPutAwayRulesRepository _putAwayRulesRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILocationMasterService _locationMasterService;
        private readonly IStorageProductService _storageProductService;
        private readonly IStockInService _stockInService;
        private readonly IStockInRepository _stockInRepository;
        private readonly IProductMasterRepository _productMasterRepository;
        private readonly ChromeContext _context;

        public StockInDetailService(IStockInDetailRepository stockInDetailRepository,
            IInventoryService inventoryService,
            IInventoryRepository inventoryRepository,
            IPutAwayRulesRepository putAwayRulesRepository,
            ILocationMasterService locationMasterService,
            IStorageProductService storageProductService,
            IStockInService stockInService,
            IStockInRepository stockInRepository,
            IProductMasterRepository productMasterRepository,
            ChromeContext context)
        {
            _stockInDetailRepository = stockInDetailRepository;
            _inventoryService = inventoryService;
            _inventoryRepository = inventoryRepository;
            _putAwayRulesRepository = putAwayRulesRepository;
            _locationMasterService = locationMasterService;
            _storageProductService = storageProductService;
            _stockInService = stockInService;
            _stockInRepository = stockInRepository;
            _productMasterRepository = productMasterRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddStockInDetail(StockInDetailRequestDTO stockInDetail)
        {
            if (stockInDetail == null) return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            if (string.IsNullOrEmpty(stockInDetail.StockInCode) || string.IsNullOrEmpty(stockInDetail.ProductCode))
                return new ServiceResponse<bool>(false, "Mã nhập kho và mã sản phẩm không được để trống");
            var stockIn = await _stockInRepository.GetStockInWithCode(stockInDetail.StockInCode);
            if (stockIn == null)
            {
                return new ServiceResponse<bool>(false, "Phiếu nhập không tồn tại");
            }
            var stockInDetailRequest = new StockInDetail
            {
                StockInCode = stockInDetail.StockInCode,
                ProductCode = stockInDetail.ProductCode,
                Demand = stockInDetail.Demand,
                LotNo = "RCV-" + stockIn.OrderDeadline!.Value.ToString("yyyyMMdd") + stockIn.SupplierCode + stockInDetail.ProductCode,
                Quantity = 0,
            };
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _stockInDetailRepository.AddAsync(stockInDetailRequest, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm sản phẩm để nhập thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi thêm sản phẩm nhập kho: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> CheckAndUpdateBackOrderStatus(string stockInCode)
        {
            if (string.IsNullOrEmpty(stockInCode))
                return new ServiceResponse<bool>(false, "Mã nhập kho không được để trống");

            // Giải mã stockInCode
            string decodedStockInCode = Uri.UnescapeDataString(stockInCode);

            // Lấy stockIn gốc
            var stockInHeader = await _context.StockIns.FirstOrDefaultAsync(x => x.StockInCode == decodedStockInCode);
            if (stockInHeader == null)
                return new ServiceResponse<bool>(false, "Phiếu nhập kho không tồn tại");

            // Lấy tất cả stockInDetails của stockInCode chính 
            var allStockInDetails = await _context.StockInDetails
                .Where(x => x.StockInCode == decodedStockInCode)
                .ToListAsync();
            if (allStockInDetails.Count == 0)
                return new ServiceResponse<bool>(false, "Không có sản phẩm trong phiếu nhập");
            bool allCompleted = allStockInDetails.All(x => x.Quantity >= x.Demand);

            if (allCompleted)
            {
                stockInHeader.StatusId = 3;
                _context.StockIns.Update(stockInHeader);
                await _context.SaveChangesAsync();
                return new ServiceResponse<bool>(true, "Phiếu nhập kho đã được hoàn tất");
            }
            return new ServiceResponse<bool>(true, "Chưa đủ số lượng để hoàn tất phiếu nhập kho");
        }

        public async Task<ServiceResponse<bool>> ConfirmStockIn(string stockInCode)
        {
            if (string.IsNullOrEmpty(stockInCode))
            {
                return new ServiceResponse<bool>(false, "Mã nhập kho không được để trống");
            }

            // Giải mã stockInCode
            string decodedStockInCode = Uri.UnescapeDataString(stockInCode);

            var lstStockInDetails = await _stockInDetailRepository.GetAllStockInDetails(decodedStockInCode).ToListAsync();
            if (lstStockInDetails.Count == 0)
            {
                return new ServiceResponse<bool>(false, "Không có sản phẩm để cập nhật tồn kho");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Preload PutAwayRules
                    var putAwayRules = (await _putAwayRulesRepository.GetAllPutAwayRules(1, int.MaxValue))
                                        .ToDictionary(x => x.ProductCode!, x => x);

                    // Preload StorageProducts
                    var storageProductIds = lstStockInDetails.Select(x => $"SP_{x.ProductCode}").Distinct().ToList();
                    var storageProducts = await _context.StorageProducts
                        .Where(x => storageProductIds.Contains(x.StorageProductId))
                        .ToDictionaryAsync(x => x.StorageProductId, x => x);

                    // Preload LocationMasters
                    var warehouseCode = lstStockInDetails.First().StockInCodeNavigation.WarehouseCode!;
                    var locationCodes = lstStockInDetails.Select(x =>
                    {
                        if (putAwayRules.ContainsKey(x.ProductCode))
                            return putAwayRules[x.ProductCode].LocationCode!;
                        else
                            return $"{warehouseCode}/VIRTUAL_LOC/{x.ProductCode}";
                    }).Distinct().ToList();

                    var locationMasters = await _context.LocationMasters
                        .Where(x => x.WarehouseCode == warehouseCode && locationCodes.Contains(x.LocationCode))
                        .ToDictionaryAsync(x => x.LocationCode, x => x);

                    // Preload ProductMasters
                    var productCodes = lstStockInDetails.Select(x => x.ProductCode).Distinct().ToList();
                    var productMasters = await _context.ProductMasters
                        .Where(x => productCodes.Contains(x.ProductCode))
                        .ToDictionaryAsync(x => x.ProductCode, x => x);

                    // Preload Inventories with client-side filtering
                    var inventoryKeys = lstStockInDetails.Select(x =>
                        new { LocationCode = putAwayRules.ContainsKey(x.ProductCode) ? putAwayRules[x.ProductCode].LocationCode! : $"{warehouseCode}/VIRTUAL_LOC/{x.ProductCode}", x.ProductCode, x.LotNo }
                    ).Distinct().ToList();

                    var inventories = _context.Inventories
                        .Where(x => x.WarehouseCode == warehouseCode)
                        .AsEnumerable() // Switch to client-side evaluation
                        .Where(x => inventoryKeys.Any(k => k.LocationCode == x.LocationCode && k.ProductCode == x.ProductCode && k.LotNo == x.Lotno))
                        .ToList();

                    foreach (var item in lstStockInDetails)
                    {
                        string locationCode;
                        if (putAwayRules.TryGetValue(item.ProductCode, out var rule))
                        {
                            locationCode = rule.LocationCode!;
                        }
                        else
                        {
                            locationCode = $"{warehouseCode}/VIRTUAL_LOC/{item.ProductCode}";

                            // StorageProduct
                            var storageProductId = $"SP_{item.ProductCode}";
                            if (!storageProducts.TryGetValue(storageProductId, out var storageProduct))
                            {
                                // Lấy BaseQuantity từ ProductMaster để tính MaxQuantity theo mét
                                var baseQuantity = productMasters.ContainsKey(item.ProductCode) ? (productMasters[item.ProductCode].BaseQuantity ?? 1) : 1;
                                var maxQuantityInBaseUOM = item.Quantity * baseQuantity;

                                storageProduct = new StorageProduct
                                {
                                    StorageProductId = storageProductId,
                                    StorageProductName = $"Định mức ảo cho {item.ProductCode}",
                                    ProductCode = item.ProductCode,
                                    MaxQuantity = maxQuantityInBaseUOM // Lưu theo mét
                                };
                                _context.StorageProducts.Add(storageProduct);
                                storageProducts.Add(storageProductId, storageProduct);
                            }
                            else
                            {
                                // Cập nhật MaxQuantity theo mét
                                var baseQuantity = productMasters.ContainsKey(item.ProductCode) ? (productMasters[item.ProductCode].BaseQuantity ?? 1) : 1;
                                storageProduct.MaxQuantity += item.Quantity * baseQuantity;
                                _context.StorageProducts.Update(storageProduct);
                            }

                            // Virtual Location
                            if (!locationMasters.ContainsKey(locationCode))
                            {
                                var newLocation = new LocationMaster
                                {
                                    LocationCode = locationCode,
                                    LocationName = $"Vùng ảo cho {item.ProductCode}",
                                    WarehouseCode = warehouseCode,
                                    StorageProductId = storageProductId
                                };
                                _context.LocationMasters.Add(newLocation);
                                locationMasters.Add(locationCode, newLocation);
                            }
                        }

                        // Inventory
                        var existingInventory = inventories.FirstOrDefault(x =>
                            x.WarehouseCode == warehouseCode &&
                            x.LocationCode == locationCode &&
                            x.ProductCode == item.ProductCode &&
                            x.Lotno == item.LotNo
                        );

                        var inventoryRequestDTO = new InventoryRequestDTO
                        {
                            WarehouseCode = warehouseCode,
                            LocationCode = locationCode,
                            ProductCode = item.ProductCode,
                            LotNo = item.LotNo!,
                            Quantity = item.Quantity // Quantity theo thanh
                        };

                        ServiceResponse<bool> inventoryResult;
                        if (existingInventory == null)
                        {
                            // Thêm mới inventory
                            inventoryResult = await _inventoryService.AddInventory(inventoryRequestDTO, saveChanges: false);
                        }
                        else
                        {
                            // Cập nhật inventory
                            inventoryResult = await _inventoryService.UpdateInventoryAsync(inventoryRequestDTO, saveChanges: false);
                        }

                        if (!inventoryResult.Success)
                        {
                            throw new Exception($"Lỗi khi xử lý tồn kho cho sản phẩm {item.ProductCode}: {inventoryResult.Message}");
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật tồn kho thành công.");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xác nhận tồn kho: {ex.Message}");
                }
            }
        }


        public async Task<ServiceResponse<bool>> CreateBackOrder(string stockInCode, string backOrderDescription)
        {
            if (string.IsNullOrEmpty(stockInCode))
                return new ServiceResponse<bool>(false, "Mã nhập kho không được để trống");

            // Giải mã stockInCode
            string decodedStockInCode = Uri.UnescapeDataString(stockInCode);

            var lstStockInDetails = await _stockInDetailRepository.GetAllStockInDetails(decodedStockInCode).ToListAsync();
            if (lstStockInDetails.Count == 0)
                return new ServiceResponse<bool>(false, "Không có sản phẩm trong phiếu nhập");

            var itemsToBackOrder = lstStockInDetails.Where(item => item.Quantity < item.Demand).ToList();
            if (itemsToBackOrder.Count == 0)
                return new ServiceResponse<bool>(false, "Không có sản phẩm cần tạo backorder");

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Lấy thông tin stock in gốc
                    var stockInHeader = itemsToBackOrder.First().StockInCodeNavigation;

                    // Tạo StockIn Backorder mới
                    var backOrderCode = "BackOrder/" + decodedStockInCode;
                    var newStockIn = new StockIn
                    {
                        StockInCode = backOrderCode,
                        OrderTypeCode = stockInHeader.OrderTypeCode,
                        WarehouseCode = stockInHeader.WarehouseCode,
                        SupplierCode = stockInHeader.SupplierCode,
                        Responsible = stockInHeader.Responsible,
                        OrderDeadline = stockInHeader.OrderDeadline,
                        StatusId = 1,
                        StockInDescription = backOrderDescription
                    };

                    await _stockInRepository.AddAsync(newStockIn, saveChanges: false);

                    // Thêm các chi tiết sản phẩm thiếu vào backorder
                    foreach (var item in itemsToBackOrder)
                    {
                        var quantityDifference = item.Demand - item.Quantity;

                        var backOrderDetail = new StockInDetail
                        {
                            StockInCode = backOrderCode,
                            ProductCode = item.ProductCode,
                            LotNo = item.LotNo,
                            Demand = quantityDifference,
                            Quantity = 0
                        };

                        item.Demand = item.Quantity;

                        await _stockInDetailRepository.AddAsync(backOrderDetail, saveChanges: false);
                        await _stockInDetailRepository.UpdateAsync(item, saveChanges: false);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ServiceResponse<bool>(true, "Đã tạo lệnh nhập kho trở lại (backorder)");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                            return new ServiceResponse<bool>(false, "Dữ liệu đã tồn tại");
                        if (error.Contains("foreign key"))
                            return new ServiceResponse<bool>(false, "Dữ liệu tham chiếu không đúng");
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi tạo backorder: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteStockInDetail(string stockInCode, string productCode)
        {
            if (string.IsNullOrEmpty(stockInCode) || string.IsNullOrEmpty(productCode))
            {
                return new ServiceResponse<bool>(false, "Mã nhập kho và mã sản phẩm không được để trống");
            }

            // Giải mã stockInCode
            string decodedStockInCode = Uri.UnescapeDataString(stockInCode);

            var existingStockInDetail = await _stockInDetailRepository.GetStockInDetailWithCode(decodedStockInCode, productCode);
            if (existingStockInDetail == null)
            {
                return new ServiceResponse<bool>(false, "Sản phẩm để nhập kho không tồn tại");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    Expression<Func<StockInDetail, bool>> predicate = x => x.StockInCode == decodedStockInCode && x.ProductCode == productCode;
                    await _stockInDetailRepository.DeleteFirstByConditionAsync(predicate);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa sản phẩm nhập kho thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa sản phẩm nhập kho vì có dữ liệu tham chiếu.");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa sản phẩm nhập kho: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<StockInDetailResponseDTO>>> GetAllStockInDetails(string stockInCode, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(stockInCode))
            {
                return new ServiceResponse<PagedResponse<StockInDetailResponseDTO>>(false, "Mã phiếu nhập kho không được để trống");
            }

            // Giải mã stockInCode
            string decodedStockInCode = Uri.UnescapeDataString(stockInCode);

            var query = _stockInDetailRepository.GetAllStockInDetails(decodedStockInCode);
            var lstStockInDetails = await query
                                    .Select(x => new StockInDetailResponseDTO
                                    {
                                        StockInCode = x.StockInCode,
                                        ProductCode = x.ProductCode,
                                        ProductName = x.ProductCodeNavigation.ProductName!,
                                        LotNo = x.LotNo,
                                        Demand = x.Demand,
                                        Quantity = x.Quantity,
                                    })
                                    .OrderBy(x => x.ProductCode)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();
            var totalItems = await query.CountAsync();
            var pagedResponse = new PagedResponse<StockInDetailResponseDTO>(lstStockInDetails, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<StockInDetailResponseDTO>>(true, "Lấy danh sách nhập kho thành công", pagedResponse);
        }

        public async Task<ServiceResponse<List<ProductMasterResponseDTO>>> GetListProductToSI()
        {
            var lstProduct = await _productMasterRepository.GetAllProduct(1, int.MaxValue);
            var lstProductForSI = lstProduct.Where(p => p.CategoryId != "SFG")
                                            .Select(p => new ProductMasterResponseDTO
                                            {
                                                ProductCode = p.ProductCode,
                                                ProductName = p.ProductName,
                                                ProductDescription = p.ProductDescription,
                                                ProductImage = p.ProductImage,
                                                CategoryId = p.CategoryId,
                                                CategoryName = p.Category?.CategoryName ?? "Không có danh mục",
                                                BaseQuantity = p.BaseQuantity,
                                                Uom = p.Uom,
                                                BaseUom = p.BaseUom,
                                                Valuation = (float?)p.Valuation,
                                            }).ToList();

            return new ServiceResponse<List<ProductMasterResponseDTO>>(true, "Lấy danh sách sản phẩm thành công", lstProductForSI);
        }

        public async Task<ServiceResponse<bool>> UpdateStockInDetail(StockInDetailRequestDTO stockInDetail)
        {
            if (stockInDetail == null) 
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");

            // Giải mã stockInCode
            string decodedStockInCode = Uri.UnescapeDataString(stockInDetail.StockInCode);

            var existingStockInDetail = await _stockInDetailRepository.GetStockInDetailWithCode(decodedStockInCode, stockInDetail.ProductCode);
            if (existingStockInDetail == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy sản phẩm để nhập kho");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Cập nhật số lượng
                    existingStockInDetail.Quantity += stockInDetail.Quantity;
                    await _stockInDetailRepository.UpdateAsync(existingStockInDetail, saveChanges: false);

                    // Kiểm tra trạng thái hoàn tất của phiếu nhập kho
                    var allStockInDetails = await _context.StockInDetails
                        .Where(x => x.StockInCode == decodedStockInCode)
                        .ToListAsync();
                    bool allCompleted = allStockInDetails.All(x => x.Quantity >= x.Demand);

                    var stockInHeader = await _context.StockIns
                        .FirstOrDefaultAsync(x => x.StockInCode == decodedStockInCode);
                    if (stockInHeader != null)
                    {
                        if (allCompleted)
                        {
                            stockInHeader.StatusId = 3; // Hoàn tất
                        }
                        else
                        {
                            stockInHeader.StatusId = 2; // Đang xử lý
                        }
                        _context.StockIns.Update(stockInHeader);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, allCompleted ? 
                        "Cập nhật số lượng sản phẩm và hoàn tất phiếu nhập kho thành công" : 
                        "Cập nhật số lượng sản phẩm và đặt trạng thái đang xử lý thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật số lượng sản phẩm nhập kho: {ex.Message}");
                }
            }
        }
    }
}