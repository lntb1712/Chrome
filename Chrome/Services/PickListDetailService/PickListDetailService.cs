using Chrome.DTO;
using Chrome.DTO.InventoryDTO;
using Chrome.DTO.PickListDetailDTO;
using Chrome.Models;
using Chrome.Repositories.PickListDetailRepository;
using Chrome.Repositories.ProductMasterRepository;
using Chrome.Services.InventoryService;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Chrome.Services.PickListDetailService
{
    public class PickListDetailService : IPickListDetailService
    {
        private readonly IPickListDetailRepository _pickListDetailRepository;
        private readonly IProductMasterRepository _productMasterRepository;
        private readonly IInventoryService _inventoryService;
        private readonly ChromeContext _context;

        public PickListDetailService(IPickListDetailRepository pickListDetailRepository,IProductMasterRepository productMasterRepository, IInventoryService inventoryService, ChromeContext context)
        {
            _pickListDetailRepository = pickListDetailRepository ?? throw new ArgumentNullException(nameof(pickListDetailRepository));
            _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
            _productMasterRepository = productMasterRepository;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ServiceResponse<PagedResponse<PickListDetailResponseDTO>>> GetAllPickListDetailsAsync(string[] warehouseCodes, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _pickListDetailRepository.GetAllPickListDetailsAsync(warehouseCodes);
                var totalItems = await query.CountAsync();
                var pickListDetails = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(pd => new PickListDetailResponseDTO
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
                                               .Where(x=>x.LocationCode == pd.LocationCode)
                                               .Select (x => x.LocationName)
                                               .FirstOrDefault()
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<PickListDetailResponseDTO>(pickListDetails, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<PickListDetailResponseDTO>>(true, "Lấy danh sách chi tiết pick list thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<PickListDetailResponseDTO>>(false, $"Lỗi khi lấy danh sách chi tiết pick list: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<PickListDetailResponseDTO>>> GetPickListDetailsByPickNoAsync(string pickNo, int page = 1, int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(pickNo))
                {
                    return new ServiceResponse<PagedResponse<PickListDetailResponseDTO>>(false, "Mã pick list không được để trống");
                }
                string decodedPickNo = Uri.UnescapeDataString(pickNo);
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _pickListDetailRepository.GetPickListDetailsByPickNoAsync(decodedPickNo);
                var totalItems = await query.CountAsync();
                var pickListDetails = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(pd => new PickListDetailResponseDTO
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
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<PickListDetailResponseDTO>(pickListDetails, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<PickListDetailResponseDTO>>(true, "Lấy chi tiết pick list theo mã thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<PickListDetailResponseDTO>>(false, $"Lỗi khi lấy chi tiết pick list theo mã: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<PickListDetailResponseDTO>>> SearchPickListDetailsAsync(string[] warehouseCodes, string pickNo, string textToSearch, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                string decodedPickNo = Uri.UnescapeDataString(pickNo);
                var query = _pickListDetailRepository.SearchPickListDetailsAsync(warehouseCodes, decodedPickNo, textToSearch);
                var totalItems = await query.CountAsync();
                var pickListDetails = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(pd => new PickListDetailResponseDTO
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
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<PickListDetailResponseDTO>(pickListDetails, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<PickListDetailResponseDTO>>(true, "Tìm kiếm chi tiết pick list thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<PickListDetailResponseDTO>>(false, $"Lỗi khi tìm kiếm chi tiết pick list: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> UpdatePickListDetail(PickListDetailRequestDTO pickListDetail)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                string decodedPickNo = Uri.UnescapeDataString(pickListDetail.PickNo);
                if (pickListDetail == null || string.IsNullOrEmpty(pickListDetail.PickNo) || string.IsNullOrEmpty(pickListDetail.ProductCode))
                {
                    return new ServiceResponse<bool>(false, "Dữ liệu đầu vào không hợp lệ. PickNo và ProductCode là bắt buộc.");
                }
                var existingDetail = await _context.PickListDetails.FirstOrDefaultAsync(x => x.PickNo == decodedPickNo && x.ProductCode == pickListDetail.ProductCode);

                if (existingDetail == null)
                {
                    return new ServiceResponse<bool>(false, "Chi tiết pick list không tồn tại.");
                }
                var product = await _productMasterRepository.GetProductMasterWithProductCode(pickListDetail.ProductCode);
                if (product == null)
                {
                    return new ServiceResponse<bool>(false, "Không tìm thấy sản phẩm");
                }



                //cập nhật tồn kho kho đi (-)
                var quantityDiff = (pickListDetail.Quantity - existingDetail.Quantity)/product.BaseQuantity;
                if (quantityDiff > 0)
                {
                    var pickList = await _context.PickLists
                        .Include(x => x.ReservationCodeNavigation)
                        .FirstOrDefaultAsync(p => p.PickNo == pickListDetail.PickNo);
                    if (pickList == null  || string.IsNullOrEmpty(pickListDetail.LocationCode))
                    {
                        return new ServiceResponse<bool>(false, " PickList hoặc thông tin WarehouseCode/LocationCode không hợp lệ");
                    }

                    var inventoryRequest = new InventoryRequestDTO
                    {
                        LocationCode = pickListDetail.LocationCode,
                        LotNo = pickListDetail.LotNo!,
                        ProductCode = pickListDetail.ProductCode,
                        Quantity = -quantityDiff,
                    };
                    existingDetail.Quantity = pickListDetail.Quantity;
                    if (pickList.ReservationCodeNavigation!.OrderTypeCode!.StartsWith("SO"))
                    {
                        var stockOutDetail = await _context.StockOutDetails
                                                        .FirstOrDefaultAsync(x => x.StockOutCode == pickListDetail.PickNo.Substring(5) && x.ProductCode == pickListDetail.ProductCode);
                        if (stockOutDetail == null)
                        {
                            return new ServiceResponse<bool>(false, "Không tìm thấy chi tiết phiếu xuất");
                        }
                        stockOutDetail!.Quantity = existingDetail.Quantity;
                        _context.StockOutDetails.Update(stockOutDetail);
                    }
                    else if (pickList.ReservationCodeNavigation!.OrderTypeCode!.StartsWith("TF"))
                    {
                        var orderCode = pickListDetail.PickNo.Substring(5);
                        int lastUnderscoreIndex = orderCode.LastIndexOf('_');
                        string transferCode = lastUnderscoreIndex != -1 ? orderCode.Substring(0, lastUnderscoreIndex) : orderCode;
                        var transferDetail = await _context.TransferDetails
                                                           .FirstOrDefaultAsync(x => x.TransferCode == transferCode && x.ProductCode == pickListDetail.ProductCode);
                        if (transferDetail == null)
                        {
                            return new ServiceResponse<bool>(false, "Không tìm thấy chi tiết chuyển kho");
                        }
                        transferDetail!.QuantityInBounded = pickListDetail.Quantity;
                        _context.TransferDetails.Update(transferDetail);
                    }
                    await _inventoryService.UpdateInventoryAsync(inventoryRequest, saveChanges: false);
                }
                existingDetail.Quantity = pickListDetail.Quantity;
                //Cập nhật trạng thái đang thực hiện cho picklist
                if (existingDetail.Quantity > 0 && existingDetail.Quantity < existingDetail.Demand)
                {
                    var pickList = await _context.PickLists
                        .Include(x => x.ReservationCodeNavigation)
                        .FirstOrDefaultAsync(pl => pl.PickNo == pickListDetail.PickNo);

                    if (pickList == null)
                    {
                        return new ServiceResponse<bool>(false, "Không tìm thấy mã phiếu lấy");
                    }
                    pickList.StatusId = 2;
                    _context.PickLists.Update(pickList);

                    var reservation = await _context.Reservations
                            .FirstOrDefaultAsync(x => x.ReservationCode == pickList.ReservationCode);
                    if (reservation == null)
                    {
                        return new ServiceResponse<bool>(false, "Không tìm thấy lệnh đặt chỗ");
                    }
                    reservation.StatusId = 2;
                    _context.Reservations.Update(reservation);

                    if (pickList.ReservationCodeNavigation!.OrderTypeCode!.StartsWith("SO"))
                    {

                        var stockOut = await _context.StockOuts
                                                 .FirstOrDefaultAsync(x => x.StockOutCode == pickListDetail.PickNo.Substring(4));
                        if (stockOut == null)
                        {
                            return new ServiceResponse<bool>(false, "Không tìm thấy phiếu xuất");
                        }
                        stockOut.StatusId = 2;
                        _context.StockOuts.Update(stockOut);
                    }
                }

                //cập nhật trạng thái hoàn thành cho picklist và trạng thái đang thực hiện cho putaway
                else if (existingDetail.Quantity == existingDetail.Demand)
                {
                    var pickList = await _context.PickLists
                        .Include(x => x.ReservationCodeNavigation)
                       .FirstOrDefaultAsync(pl => pl.PickNo == pickListDetail.PickNo);

                    if (pickList == null)
                    {
                        return new ServiceResponse<bool>(false, "Không tìm thấy mã phiếu lấy");
                    }
                    pickList.StatusId = 2;
                    _context.PickLists.Update(pickList);

                    var reservation = await _context.Reservations
                           .FirstOrDefaultAsync(x => x.ReservationCode == pickList.ReservationCode);
                    if (reservation == null)
                    {
                        return new ServiceResponse<bool>(false, "Không tìm thấy lệnh đặt chỗ");
                    }
                    reservation.StatusId = 3;
                    _context.Reservations.Update(reservation);

                    string pickNo = existingDetail.PickNo!;
                    if (pickList.ReservationCodeNavigation!.OrderTypeCode!.StartsWith("MV"))
                    {

                        string movementCode = pickNo.StartsWith("PICK_") ? pickNo.Substring(5) : pickNo;
                        // Tìm movement dựa trên movementCode
                        var movement = await _context.Movements
                            .FirstOrDefaultAsync(m => m.MovementCode == movementCode);

                        if (movement == null)
                        {
                            return new ServiceResponse<bool>(false, $"Không tìm thấy Movement với mã {movementCode}.");
                        }
                        movement.StatusId = 2;
                        _context.Movements.Update(movement);
                    }
                    else if (pickList.ReservationCodeNavigation!.OrderTypeCode!.StartsWith("TF"))
                    {
                        var orderCode = pickListDetail.PickNo.Substring(5);
                        int lastUnderscoreIndex = orderCode.LastIndexOf('_');
                        string transferCode = lastUnderscoreIndex != -1 ? orderCode.Substring(0, lastUnderscoreIndex) : orderCode;
                        // Tìm movement dựa trên movementCode
                        var transfer = await _context.Transfers
                            .FirstOrDefaultAsync(m => m.TransferCode == transferCode);

                        if (transfer == null)
                        {
                            return new ServiceResponse<bool>(false, $"Không tìm thấy lệnh chuyển kho với mã {transfer}.");
                        }
                        transfer.StatusId = 2;
                        _context.Transfers.Update(transfer);
                    }

                }
                var allDetails = await _context.PickListDetails
                        .Where(x => x.PickNo == pickListDetail.PickNo)
                        .ToListAsync();

                bool allCompleted = allDetails.All(x => x.Quantity >= x.Demand);
                if (allCompleted)
                {
                    var pickListToUpdate = await _context.PickLists
                        .FirstOrDefaultAsync(pl => pl.PickNo == pickListDetail.PickNo);
                    if (pickListToUpdate != null)
                    {
                        pickListToUpdate.StatusId = 3; // trạng thái hoàn tất toàn bộ
                        _context.PickLists.Update(pickListToUpdate);
                    }
                }


                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Cập nhật chi tiết pick list thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi cập nhật chi tiết pick list: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật chi tiết pick list: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> DeletePickListDetail(string pickNo, string productCode)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(pickNo) || string.IsNullOrEmpty(productCode))
                {
                    return new ServiceResponse<bool>(false, "PickNo và ProductCode không được để trống.");
                }
                string decodedPickNo = Uri.UnescapeDataString(pickNo);

                var pickListDetail = await _context.PickListDetails
                    .FirstOrDefaultAsync(pd => pd.PickNo == decodedPickNo && pd.ProductCode == productCode);

                if (pickListDetail == null)
                {
                    return new ServiceResponse<bool>(false, "Chi tiết pick list không tồn tại.");
                }

                Expression<Func<PickListDetail, bool>> predicate = x => x.ProductCode == productCode && x.PickNo == decodedPickNo;
                await _pickListDetailRepository.DeleteFirstByConditionAsync(predicate);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Xóa chi tiết pick list thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi xóa chi tiết pick list: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi xóa chi tiết pick list: {ex.Message}");
            }
        }
    }
}