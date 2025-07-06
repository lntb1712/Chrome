using Chrome.DTO;
using Chrome.DTO.InventoryDTO;
using Chrome.DTO.LocationMasterDTO;
using Chrome.DTO.ProductMasterDTO;
using Chrome.DTO.PutAwayDTO;
using Chrome.DTO.StockInDetailDTO;
using Chrome.DTO.StockInDTO;
using Chrome.DTO.StorageProductDTO;
using Chrome.Models;
using Chrome.Repositories.InventoryRepository;
using Chrome.Repositories.ProductMasterRepository;
using Chrome.Repositories.PutawayRepository;
using Chrome.Repositories.PutAwayRulesRepository;
using Chrome.Repositories.StockInDetailRepository;
using Chrome.Repositories.StockInRepository;
using Chrome.Repositories.StorageProductRepository;
using Chrome.Services.InventoryService;
using Chrome.Services.LocationMasterService;
using Chrome.Services.PutAwayService;
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
        private readonly IPutAwayService _putAwayService;
        private readonly IPutAwayRepository _putAwayRepository;
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
            IPutAwayService putAwayService,
            IPutAwayRepository putAwayRepository,
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
            _putAwayService = putAwayService;
            _putAwayRepository = putAwayRepository;
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
                LotNo = "RCV-" + stockInDetail.StockInCode + "-" + stockInDetail.ProductCode,
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
                    // Tính số lượng mới sau khi cập nhật
                    var newQuantity = existingStockInDetail.Quantity + stockInDetail.Quantity;
                    if (newQuantity < 0)
                    {
                        return new ServiceResponse<bool>(false, "Số lượng không thể âm");
                    }

                    // Cập nhật số lượng StockInDetail
                    existingStockInDetail.Quantity = newQuantity;
                    await _stockInDetailRepository.UpdateAsync(existingStockInDetail, saveChanges: false);

                    // Tạo hoặc cập nhật PutAway khi có số lượng hợp lệ
                    if (existingStockInDetail.Quantity > 0 && existingStockInDetail.Quantity <= existingStockInDetail.Demand)
                    {
                        var stockIn = await _stockInRepository.GetStockInWithCode(existingStockInDetail.StockInCode);
                        var stockInDetailResponse = await _stockInDetailRepository.GetStockInDetailWithCode(stockIn.StockInCode, stockInDetail.ProductCode);

                        // Preload PutAwayRules
                        var putAwayRulesPaged = await _putAwayRulesRepository.GetAllPutAwayRules(1, int.MaxValue);
                        var putAwayRulesList = putAwayRulesPaged
                            .Where(x => x.ProductCode == stockInDetail.ProductCode && x.LocationCodeNavigation!.WarehouseCode == stockIn.WarehouseCode)
                            .ToList();

                        // Load ProductMasters, LocationMasters, and StorageProducts
                        var productMasters = (await _productMasterRepository.GetAllProduct(1, int.MaxValue))
                            .GroupBy(p => p.ProductCode!)
                            .ToDictionary(g => g.Key, g => g.First());
                        var locationMasters = await _context.LocationMasters
                            .Where(l => l.WarehouseCode == stockIn.WarehouseCode)
                            .ToDictionaryAsync(l => l.LocationCode!, l => l);
                        var storageProducts = await _context.StorageProducts
                            .ToDictionaryAsync(sp => sp.StorageProductId!, sp => sp);

                        // Tính tổng số lượng tồn kho hiện tại tại các vị trí
                        var inventoryQuantities = await _inventoryRepository.GetInventoryByProductCodeAsync(stockInDetail.ProductCode, stockIn.WarehouseCode!)
                            .GroupBy(i => i.LocationCode)
                            .Select(g => new { LocationCode = g.Key, TotalQuantity = g.Sum(i => i.Quantity ?? 0) })
                            .ToListAsync();

                        string? selectedLocationCode = null;
                        double quantityToPut = (double)newQuantity!;

                        // Tìm vị trí phù hợp từ PutAwayRules
                        if (putAwayRulesList.Any())
                        {
                            var sortedRules = putAwayRulesList
                                .Where(r => locationMasters.ContainsKey(r.LocationCode!) && storageProducts.ContainsKey(locationMasters[r.LocationCode!].StorageProductId!))
                                .OrderBy(r => inventoryQuantities.FirstOrDefault(q => q.LocationCode == r.LocationCode)?.TotalQuantity ?? 0);

                            foreach (var rule in sortedRules)
                            {
                                var locationCode = rule.LocationCode!;
                                var storageProductId = locationMasters[locationCode].StorageProductId!;
                                var storageProduct = storageProducts[storageProductId];
                                var currentQuantity = inventoryQuantities.FirstOrDefault(q => q.LocationCode == locationCode)?.TotalQuantity ?? 0;
                                var maxQuantity = storageProduct.MaxQuantity ?? int.MaxValue;
                                var availableSpace = maxQuantity - currentQuantity;

                                if (availableSpace >= quantityToPut)
                                {
                                    selectedLocationCode = locationCode;
                                    break;
                                }
                            }
                        }

                        // Nếu không tìm được vị trí thực, thử vị trí ảo
                        if (selectedLocationCode == null)
                        {
                            var locationCode = $"{stockIn.WarehouseCode}/VIRTUAL_LOC/{stockInDetail.ProductCode}";
                            var storageProductId = $"SP_{stockInDetail.ProductCode}";

                            if (!storageProducts.TryGetValue(storageProductId, out var storageProduct))
                            {
                                var baseQuantity = productMasters.ContainsKey(stockInDetail.ProductCode) ? (productMasters[stockInDetail.ProductCode].BaseQuantity ?? 1) : 1;
                                var maxQuantityInBaseUOM = stockInDetail.Demand * baseQuantity;

                                storageProduct = new StorageProduct
                                {
                                    StorageProductId = storageProductId,
                                    StorageProductName = $"Định mức ảo cho {stockInDetail.ProductCode}",
                                    ProductCode = stockInDetail.ProductCode,
                                    MaxQuantity = maxQuantityInBaseUOM
                                };
                                _context.StorageProducts.Add(storageProduct);
                                storageProducts.Add(storageProductId, storageProduct);
                            }
                            else
                            {
                                var baseQuantity = productMasters.ContainsKey(stockInDetail.ProductCode) ? (productMasters[stockInDetail.ProductCode].BaseQuantity ?? 1) : 1;
                                storageProduct.MaxQuantity += stockInDetail.Demand * baseQuantity;
                                _context.StorageProducts.Update(storageProduct);
                            }

                            // Tạo vị trí ảo nếu chưa tồn tại
                            if (!locationMasters.ContainsKey(locationCode))
                            {
                                var newLocation = new LocationMaster
                                {
                                    LocationCode = locationCode,
                                    LocationName = $"Vùng ảo cho {stockInDetail.ProductCode}",
                                    WarehouseCode = stockIn.WarehouseCode,
                                    StorageProductId = storageProductId
                                };
                                _context.LocationMasters.Add(newLocation);
                                locationMasters.Add(locationCode, newLocation);
                                await _context.SaveChangesAsync();
                            }

                            // Kiểm tra định mức của vị trí ảo
                            var currentLocationQuantity = inventoryQuantities.FirstOrDefault(q => q.LocationCode == locationCode)?.TotalQuantity ?? 0;
                            var availableSpace = storageProduct.MaxQuantity - currentLocationQuantity;

                            if (availableSpace >= quantityToPut)
                            {
                                selectedLocationCode = locationCode;
                            }
                        }

                        // Nếu không tìm được vị trí nào đủ định mức, trả về lỗi
                        if (selectedLocationCode == null)
                        {
                            await transaction.RollbackAsync();
                            return new ServiceResponse<bool>(false, $"Không tìm được vị trí có đủ định mức để cất {quantityToPut} đơn vị sản phẩm {stockInDetail.ProductCode}");
                        }

                        // Tạo hoặc cập nhật PutAway cho vị trí được chọn
                        var putAwayCode = $"PUT_{stockIn.StockInCode}_{stockInDetail.ProductCode}";
                        var putAway = await _putAwayService.GetPutAwayByCodeAsync(putAwayCode);
                        if (putAway.Data == null)
                        {
                            var putAwayRequest = new PutAwayRequestDTO
                            {
                                PutAwayCode = putAwayCode,
                                OrderTypeCode = stockIn.OrderTypeCode,
                                LocationCode = selectedLocationCode,
                                Responsible = stockIn.Responsible,
                                StatusId = 1,
                                PutAwayDate = DateTime.Now.ToString("dd/MM/yyyy"),
                                PutAwayDescription = $"Cất hàng cho lệnh nhập kho {stockIn.StockInCode}, sản phẩm {stockInDetail.ProductCode}"
                            };
                            var putAwayResponse = await _putAwayService.AddPutAway(putAwayRequest, transaction);
                            if (!putAwayResponse.Success)
                            {
                                await transaction.RollbackAsync();
                                return new ServiceResponse<bool>(false, $"Lỗi khi tạo putaway: {putAwayResponse.Message}");
                            }
                        }
                        else
                        {
                            // Cập nhật LocationCode nếu cần
                            var existPutAway = await _putAwayRepository.GetPutAwayCodeAsync(putAwayCode);
                            var putAwayRequest = new PutAwayRequestDTO
                            {
                                PutAwayCode = putAwayCode,
                                OrderTypeCode = existPutAway.OrderTypeCode,
                                LocationCode = selectedLocationCode,
                                Responsible = stockIn.Responsible,
                                StatusId = 1,
                                PutAwayDate = DateTime.Now.ToString("dd/MM/yyyy"),
                                PutAwayDescription = $"Cất hàng cho lệnh nhập kho {stockIn.StockInCode}, sản phẩm {stockInDetail.ProductCode}"
                            };
                            var updatePutAwayResponse = await _putAwayService.UpdatePutAway(putAwayRequest, transaction);
                            if (!updatePutAwayResponse.Success)
                            {
                                await transaction.RollbackAsync();
                                return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật putaway: {updatePutAwayResponse.Message}");
                            }
                        }

                        // Tạo hoặc cập nhật PutAwayDetail
                        var putAwayDetail = new PutAwayDetail
                        {
                            PutAwayCode = putAwayCode,
                            ProductCode = stockInDetail.ProductCode,
                            LotNo = stockInDetailResponse.LotNo,
                            Demand = quantityToPut,
                            Quantity = 0 // Quantity sẽ được cập nhật sau khi cất hàng thực tế
                        };

                        var existingPutAwayDetail = await _context.PutAwayDetails
                            .FirstOrDefaultAsync(x => x.PutAwayCode == putAwayDetail.PutAwayCode && x.ProductCode == putAwayDetail.ProductCode);

                        if (existingPutAwayDetail == null)
                        {
                            await _context.PutAwayDetails.AddAsync(putAwayDetail);
                        }
                        else
                        {
                            existingPutAwayDetail.Demand = quantityToPut;
                            existingPutAwayDetail.LotNo = stockInDetailResponse.LotNo;
                            _context.PutAwayDetails.Update(existingPutAwayDetail);
                        }

                        await _context.SaveChangesAsync();
                    }

                    // Kiểm tra trạng thái hoàn tất của phiếu nhập kho
                    var allStockInDetails = await _context.StockInDetails
                        .Where(x => x.StockInCode == decodedStockInCode)
                        .ToListAsync();
                    bool allCompleted = allStockInDetails.All(x => x.Quantity >= x.Demand);

                    var stockInHeader = await _context.StockIns
                        .FirstOrDefaultAsync(x => x.StockInCode == decodedStockInCode);
                    if (stockInHeader != null)
                    {
                        if (!allCompleted)
                        {
                            stockInHeader.StatusId = 2; // Hoàn tất
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

        public async Task<ServiceResponse<bool>> ConfirmStockIn(string stockInCode)
        {
            // Validate input
            if (string.IsNullOrEmpty(stockInCode))
            {
                return new ServiceResponse<bool>(false, "Mã nhập kho không được để trống");
            }

            // Decode stockInCode
            string decodedStockInCode = Uri.UnescapeDataString(stockInCode);

            // Get stock in details
            var stockInDetails = await _stockInDetailRepository
                .GetAllStockInDetails(decodedStockInCode)
                .ToListAsync();

            if (!stockInDetails.Any())
            {
                return new ServiceResponse<bool>(false, "Không có sản phẩm để cập nhật tồn kho");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update stock quantities
                foreach (var detail in stockInDetails)
                {
                    var existingDetail = await _stockInDetailRepository
                        .GetStockInDetailWithCode(decodedStockInCode, detail.ProductCode);

                    if (existingDetail == null)
                    {
                        await transaction.RollbackAsync();
                        return new ServiceResponse<bool>(false, "Không tìm thấy sản phẩm để nhập kho");
                    }

                    existingDetail.Demand = detail.Quantity;
                    var putAwayDetail = await _context.PutAwayDetails.FirstOrDefaultAsync(x => x.PutAwayCode.Contains(stockInCode) && x.ProductCode == existingDetail.ProductCode);
                    if(putAwayDetail==null)
                    {
                        return new ServiceResponse<bool>(false, "Không tìm thấy chi tiết cất hàng");
                    }    
                    putAwayDetail.Demand = putAwayDetail.Quantity;
                    _context.PutAwayDetails.Update(putAwayDetail);
                    await _stockInDetailRepository.UpdateAsync(existingDetail, saveChanges: false);
                }

                // Update stock in header status
                var allDetails = await _context.StockInDetails
                    .Where(x => x.StockInCode == decodedStockInCode)
                    .ToListAsync();

                bool isCompleted = allDetails.All(x => x.Quantity >= x.Demand);

                var stockInHeader = await _context.StockIns
                    .FirstOrDefaultAsync(x => x.StockInCode == decodedStockInCode);

                if (stockInHeader != null)
                {
                    stockInHeader.StatusId = isCompleted ? 3 : 2; // 3: Completed, 2: Processing
                    _context.StockIns.Update(stockInHeader);
                }

                // Save changes and commit
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ServiceResponse<bool>(true,
                    isCompleted
                        ? "Cập nhật số lượng và hoàn tất phiếu nhập kho thành công"
                        : "Cập nhật số lượng và đặt trạng thái đang xử lý thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                string errorMessage = dbEx.InnerException?.Message.ToLower() ?? "";

                if (errorMessage.Contains("unique") ||
                    errorMessage.Contains("duplicate") ||
                    errorMessage.Contains("primary key"))
                {
                    return new ServiceResponse<bool>(false, "Dữ liệu đã tồn tại");
                }

                if (errorMessage.Contains("foreign key"))
                {
                    return new ServiceResponse<bool>(false, "Dữ liệu tham chiếu không đúng");
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