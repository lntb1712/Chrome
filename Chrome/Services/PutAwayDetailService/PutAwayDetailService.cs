using Azure;
using Chrome.DTO;
using Chrome.DTO.InventoryDTO;
using Chrome.DTO.PutAwayDetailDTO;
using Chrome.Models;
using Chrome.Repositories.InventoryRepository;
using Chrome.Repositories.ProductMasterRepository;
using Chrome.Repositories.PutAwayDetailRepository;
using Chrome.Services.InventoryService;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Chrome.Services.PutAwayDetailService
{
    public class PutAwayDetailService : IPutAwayDetailService
    {
        private readonly IPutAwayDetailRepository _putAwayDetailRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IInventoryService _inventoryService;
        private readonly IProductMasterRepository _productMasterRepository;
        private readonly ChromeContext _context;

        public PutAwayDetailService(IPutAwayDetailRepository putAwayDetailRepository,IInventoryRepository inventoryRepository,IProductMasterRepository productMasterRepository, IInventoryService inventoryService, ChromeContext context)
        {
            _putAwayDetailRepository = putAwayDetailRepository ?? throw new ArgumentNullException(nameof(putAwayDetailRepository));
            _inventoryRepository = inventoryRepository ?? throw new ArgumentNullException(nameof(inventoryRepository));
            _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
            _productMasterRepository = productMasterRepository ?? throw new ArgumentNullException(nameof(productMasterRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>> GetAllPutAwayDetailsAsync(string[] warehouseCodes, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _putAwayDetailRepository.GetAllPutAwayDetailsAsync(warehouseCodes);
                var totalItems = await query.CountAsync();
                var putAwayDetails = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(pd => new PutAwayDetailResponseDTO
                    {
                        PutAwayCode = pd.PutAwayCode,
                        ProductCode = pd.ProductCode,
                        ProductName = pd.ProductCodeNavigation!.ProductName!,
                        LotNo = pd.LotNo,
                        Demand = pd.Demand,
                        Quantity = pd.Quantity
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<PutAwayDetailResponseDTO>(putAwayDetails, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>(true, "Lấy danh sách chi tiết putaway thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>(false, $"Lỗi khi lấy danh sách chi tiết putaway: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>> GetPutAwayDetailsByPutawayCodeAsync(string putawayNo, int page = 1, int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(putawayNo))
                {
                    return new ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>(false, "Mã putaway không được để trống");
                }

                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _putAwayDetailRepository.GetPutAwayDetailsByPutawayNoAsync(putawayNo);
                var totalItems = await query.CountAsync();
                var putAwayDetails = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(pd => new PutAwayDetailResponseDTO
                    {
                        PutAwayCode = pd.PutAwayCode,
                        ProductCode = pd.ProductCode,
                        ProductName = pd.ProductCodeNavigation!.ProductName!,
                        LotNo = pd.LotNo,
                        Demand = pd.Demand,
                        Quantity = pd.Quantity
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<PutAwayDetailResponseDTO>(putAwayDetails, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>(true, "Lấy chi tiết putaway theo mã thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>(false, $"Lỗi khi lấy chi tiết putaway theo mã: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>> SearchPutAwayDetailsAsync(string[] warehouseCodes, string putawayNo, string textToSearch, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _putAwayDetailRepository.SearchPutAwayDetailsAsync(warehouseCodes, putawayNo, textToSearch);
                var totalItems = await query.CountAsync();
                var putAwayDetails = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(pd => new PutAwayDetailResponseDTO
                    {
                        PutAwayCode = pd.PutAwayCode,
                        ProductCode = pd.ProductCode,
                        ProductName = pd.ProductCodeNavigation!.ProductName!,
                        LotNo = pd.LotNo,
                        Demand = pd.Demand,
                        Quantity = pd.Quantity
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<PutAwayDetailResponseDTO>(putAwayDetails, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>(true, "Tìm kiếm chi tiết putaway thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>(false, $"Lỗi khi tìm kiếm chi tiết putaway: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> UpdatePutAwayDetail(PutAwayDetailRequestDTO putAwayDetail)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (putAwayDetail == null || string.IsNullOrEmpty(putAwayDetail.PutAwayCode) || string.IsNullOrEmpty(putAwayDetail.ProductCode))
                {
                    return new ServiceResponse<bool>(false, "Dữ liệu đầu vào không hợp lệ. PutAwayCode và ProductCode là bắt buộc.");
                }

                var existingDetail = await _context.PutAwayDetails
                    .FirstOrDefaultAsync(pd => pd.PutAwayCode == putAwayDetail.PutAwayCode && pd.ProductCode == putAwayDetail.ProductCode);

                if (existingDetail == null)
                {
                    return new ServiceResponse<bool>(false, "Chi tiết putaway không tồn tại.");
                }

                var product = await _productMasterRepository.GetProductMasterWithProductCode(putAwayDetail.ProductCode);

                if (product == null)
                {
                    return new ServiceResponse<bool>(false, "Không tìm thấy product");
                }
                var putAway = await _context.PutAways
                        .Include(x => x.LocationCodeNavigation)
                        .FirstOrDefaultAsync(p => p.PutAwayCode == putAwayDetail.PutAwayCode);
                var quantityDiff = 0.0;
                if (putAway!.OrderTypeCode!.StartsWith("MO"))
                {
                    quantityDiff = (double)((putAwayDetail.Quantity - existingDetail.Quantity) / product.BaseQuantity)!;
                }
                else {
                    quantityDiff = (double)(putAwayDetail.Quantity - existingDetail.Quantity)!;
                }


                //cộng tồn kho nhận (+)
                if (quantityDiff > 0)
                {

                    
                    if (putAway == null || string.IsNullOrEmpty(putAway.LocationCodeNavigation!.WarehouseCode) || string.IsNullOrEmpty(putAway.LocationCode))
                    {
                        return new ServiceResponse<bool>(false, "PutAway hoặc thông tin WarehouseCode/LocationCode không hợp lệ.");
                    }
                    var inventoryRequest = new InventoryRequestDTO
                    {
                        LocationCode = putAway.LocationCode,
                        LotNo = putAwayDetail.LotNo!,
                        ProductCode = putAwayDetail.ProductCode,
                        Quantity = quantityDiff,
                    };
                    var existingInventory = await _inventoryRepository.GetInventoryWithCode( putAway.LocationCode, putAwayDetail.ProductCode, putAwayDetail.LotNo!);
                    if(existingInventory==null)
                    {
                        await _inventoryService.AddInventory(inventoryRequest, saveChanges: false);
                    }
                    else
                    {
                        await _inventoryService.UpdateInventoryAsync(inventoryRequest, saveChanges: false);
                    }

                    string putAwayCode = existingDetail.PutAwayCode;
                    string orderCode = putAwayCode.StartsWith("PUT_") ? putAwayCode.Substring(4) : putAwayCode;
                    if (putAway.OrderTypeCode!.StartsWith("TF"))
                    {
                        int lastUnderscoreIndex = orderCode.LastIndexOf('_');
                        string transferCode = lastUnderscoreIndex != -1 ? orderCode.Substring(0, lastUnderscoreIndex) : orderCode;

                        var transferDetail = await _context.TransferDetails
                                                          .FirstOrDefaultAsync(x => x.TransferCode == transferCode && x.ProductCode == putAwayDetail.ProductCode);

                        if (transferDetail == null)
                        {
                            return new ServiceResponse<bool>(false, $"Không tìm thấy chi tiết Lệnh chuyển kho với mã {transferCode}");
                        }

                        transferDetail!.QuantityOutBounded = putAwayDetail.Quantity;
                        _context.TransferDetails.Update(transferDetail);
                    }
                   



                }
                existingDetail.Quantity = putAwayDetail.Quantity;
                //Cập nhật trạng thái đang thực hiện cho putAway
                if (existingDetail.Quantity > 0 && existingDetail.Quantity < existingDetail.Demand)
                {

                    if (putAway == null)
                    {
                        return new ServiceResponse<bool>(false, "Không tìm thấy mã phiếu để hàng");
                    }
                    putAway.StatusId = 2;
                    _context.PutAways.Update(putAway);

                    string putAwayCode = existingDetail.PutAwayCode;

                    string orderCode = putAwayCode.StartsWith("PUT_") ? putAwayCode.Substring(4) : putAwayCode;


                }
                //Cập nhật trạng thái hoàn thành cho putaway
                else if (existingDetail.Quantity == existingDetail.Demand)
                {

                    if (putAway == null)
                    {
                        return new ServiceResponse<bool>(false, "Không tìm thấy mã phiếu để hàng");
                    }
                    putAway.StatusId = 2;
                    _context.PutAways.Update(putAway);

                    string putAwayCode = existingDetail.PutAwayCode;

                    string orderCode = putAwayCode.StartsWith("PUT_") ? putAwayCode.Substring(4) : putAwayCode;
                    if (putAway.OrderTypeCode!.StartsWith("MV"))
                    {
                        // Tìm movement dựa trên movementCode
                        var movement = await _context.Movements
                            .FirstOrDefaultAsync(m => m.MovementCode == orderCode);

                        if (movement == null)
                        {
                            return new ServiceResponse<bool>(false, $"Không tìm thấy Movement với mã {orderCode}.");
                        }
                        movement.StatusId = 3;
                        _context.Movements.Update(movement);
                    }
                    else if (putAway.OrderTypeCode!.StartsWith("TF"))
                    {
                        int lastUnderscoreIndex = orderCode.LastIndexOf('_');
                        string transferCode = lastUnderscoreIndex != -1 ? orderCode.Substring(0, lastUnderscoreIndex) : orderCode;

                        var transfer = await _context.Transfers
                             .FirstOrDefaultAsync(x => x.TransferCode == transferCode);

                        if (transfer == null)
                        {
                            return new ServiceResponse<bool>(false, $"Không tìm thấy Lệnh chuyển kho với mã {transferCode}");
                        }

                        transfer.StatusId = 3;
                        _context.Transfers.Update(transfer);
                    }
                }
                var allDetails = await _context.PutAwayDetails
                       .Where(x => x.PutAwayCode == putAwayDetail.PutAwayCode)
                       .ToListAsync();

                bool allCompleted = allDetails.All(x => x.Quantity >= x.Demand);
                if (allCompleted)
                {
                    var putAwayToUpdate = await _context.PutAways
                        .FirstOrDefaultAsync(pl => pl.PutAwayCode == putAwayDetail.PutAwayCode);
                    if (putAwayToUpdate != null)
                    {
                        putAwayToUpdate.StatusId = 3; // trạng thái hoàn tất toàn bộ
                        _context.PutAways.Update(putAwayToUpdate);
                    }
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Cập nhật chi tiết putaway thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi cập nhật chi tiết putaway: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật chi tiết putaway: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> DeletePutAwayDetail(string putawayNo, string productCode)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(putawayNo) || string.IsNullOrEmpty(productCode))
                {
                    return new ServiceResponse<bool>(false, "PutAwayCode và ProductCode không được để trống.");
                }

                var putAwayDetail = await _context.PutAwayDetails
                    .FirstOrDefaultAsync(pd => pd.PutAwayCode == putawayNo && pd.ProductCode == productCode);

                if (putAwayDetail == null)
                {
                    return new ServiceResponse<bool>(false, "Chi tiết putaway không tồn tại.");
                }

                _context.PutAwayDetails.Remove(putAwayDetail);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Xóa chi tiết putaway thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi xóa chi tiết putaway: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi xóa chi tiết putaway: {ex.Message}");
            }
        }

        
    }
}