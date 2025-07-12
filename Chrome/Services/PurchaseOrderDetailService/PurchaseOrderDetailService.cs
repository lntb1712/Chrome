using Chrome.DTO;
using Chrome.DTO.ProductMasterDTO;
using Chrome.DTO.PurchaseOrderDetailDTO;
using Chrome.Models;
using Chrome.Repositories.ProductMasterRepository;
using Chrome.Repositories.PurchaseOrderDetailRepository;
using Chrome.Repositories.PurchaseOrderRepository;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;

namespace Chrome.Services.PurchaseOrderDetailService
{
    public class PurchaseOrderDetailService : IPurchaseOrderDetailService
    {
        private readonly IPurchaseOrderDetailRepository _purchaseOrderDetailRepository;
        private readonly IProductMasterRepository _productMasterRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly ChromeContext _context;

        public PurchaseOrderDetailService(IPurchaseOrderDetailRepository purchaseOrderDetailRepository, IProductMasterRepository productMasterRepository, IPurchaseOrderRepository purchaseOrderRepository, ChromeContext context)
        {
            _purchaseOrderDetailRepository = purchaseOrderDetailRepository;
            _productMasterRepository = productMasterRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddPurchaseOrderDetail(PurchaseOrderDetailRequestDTO purchaseOrderDetail)
        {
            if (purchaseOrderDetail == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ.");
            }
            var purchaseOrderDetailEntity = new PurchaseOrderDetail
            {
                PurchaseOrderCode = purchaseOrderDetail.PurchaseOrderCode!,
                ProductCode = purchaseOrderDetail.ProductCode!,
                Quantity = purchaseOrderDetail.Quantity,
                QuantityReceived = 0 // Khởi tạo số lượng đã nhận là 0
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _purchaseOrderDetailRepository.AddAsync(purchaseOrderDetailEntity, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi không xác định: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> CheckAndUpdateBackOrderStatus(string purchaseOrderDetailCode)
        {
            if (string.IsNullOrEmpty(purchaseOrderDetailCode))
            {
                return new ServiceResponse<bool>(false, "Mã chi tiết đơn hàng không được để trống.");
            }

            string decodedPurchaseOrderDetailCode = Uri.UnescapeDataString(purchaseOrderDetailCode);
            var purchaseOrderHeader = await _context.PurchaseOrders.FirstOrDefaultAsync(po => po.PurchaseOrderCode == decodedPurchaseOrderDetailCode);
            if (purchaseOrderHeader == null)
            {
                return new ServiceResponse<bool>(false, "đơn hàng không tồn tại.");
            }

            var allPurchaseOrderDetails = await _purchaseOrderDetailRepository.GetAllPurchaseOrderDetailsAsync(decodedPurchaseOrderDetailCode).ToListAsync();
            if (allPurchaseOrderDetails == null || allPurchaseOrderDetails.Count == 0)
            {
                return new ServiceResponse<bool>(false, "Không có chi tiết đơn hàng nào để kiểm tra.");
            }

            bool allCompleted = allPurchaseOrderDetails.All(detail => detail.QuantityReceived >= detail.Quantity);

            if (allCompleted)
            {
                purchaseOrderHeader.StatusId = 3;
                _context.PurchaseOrders.Update(purchaseOrderHeader);
                await _context.SaveChangesAsync();
                return new ServiceResponse<bool>(true, "Tất cả chi tiết đơn hàng đã hoàn thành, cập nhật trạng thái đơn hàng thành công.");
            }
            return new ServiceResponse<bool>(true, "Không cần cập nhật trạng thái đơn hàng, vẫn còn chi tiết chưa hoàn thành.");
        }

        public async Task<ServiceResponse<bool>> ConfirmPurchaseOrderDetail(string purchaseOrderDetailCode)
        {
            if (string.IsNullOrEmpty(purchaseOrderDetailCode))
            {
                return new ServiceResponse<bool>(false,"Mã chi tiết đơn hàng không được để trống.");
            }
            string decodedPurchaseOrderDetailCode = Uri.UnescapeDataString(purchaseOrderDetailCode);
            var purchaseOrderDetail = await _purchaseOrderDetailRepository.GetAllPurchaseOrderDetailsAsync(decodedPurchaseOrderDetailCode).ToListAsync();
            if (purchaseOrderDetail == null || purchaseOrderDetail.Count == 0)
            {
                return new ServiceResponse<bool>(false, "Chi tiết đơn hàng không tồn tại.");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var detail in purchaseOrderDetail)
                    {
                        var existingDetail = await _purchaseOrderDetailRepository.GetPurchaseOrderDetailWithCode(detail.PurchaseOrderCode, detail.ProductCode);
                        if(existingDetail == null)
                        {
                            await transaction.RollbackAsync();
                            return new ServiceResponse<bool>(false, "Chi tiết đơn hàng không tồn tại.");
                        }    
                        existingDetail.Quantity = existingDetail.QuantityReceived; // Cập nhật số lượng đã nhận cho chi tiết đơn hàng
                        await _purchaseOrderDetailRepository.UpdateAsync(existingDetail, saveChanges: false);
                    }
                    var allDetails = await _context.PurchaseOrderDetails
                   .Where(x => x.PurchaseOrderCode == decodedPurchaseOrderDetailCode)
                   .ToListAsync();

                    bool isCompleted = allDetails.All(x => x.QuantityReceived >= x.Quantity);

                    var purchaseOrderHeader = await _context.PurchaseOrders
                        .FirstOrDefaultAsync(x => x.PurchaseOrderCode == decodedPurchaseOrderDetailCode);

                    if (purchaseOrderHeader != null)
                    {
                        purchaseOrderHeader.StatusId = isCompleted ? 3 : 2; // 3: Completed, 2: Processing
                        _context.PurchaseOrders.Update(purchaseOrderHeader);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true,
                    isCompleted
                        ? "Cập nhật số lượng và hoàn tất phiếu đặt hàng thành công"
                        : "Cập nhật số lượng và đặt trạng thái đang xử lý thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi không xác định: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> CreateBackOrder(string purchaseOrderDetailCode, string backOrderDescription, string dateBackOrder)
        {
            if (string.IsNullOrEmpty(purchaseOrderDetailCode))
            {
                return new ServiceResponse<bool>(false, "Mã chi tiết đơn hàng  không được để trống.");
            }

            string decodedPurchaseOrderDetailCode = Uri.UnescapeDataString(purchaseOrderDetailCode);
            var purchaseOrderDetail = await _purchaseOrderDetailRepository.GetAllPurchaseOrderDetailsAsync(decodedPurchaseOrderDetailCode).ToListAsync();
            if (purchaseOrderDetail == null)
            {
                return new ServiceResponse<bool>(false, "Chi tiết đơn hàng không tồn tại.");
            }
            var itemToBackOrder = purchaseOrderDetail.Where(x => x.QuantityReceived < x.Quantity).ToList();
            if (itemToBackOrder.Count == 0)
            {
                return new ServiceResponse<bool>(false, "Tất cả chi tiết đơn hàng đã được xác nhận.");
            }

            if (string.IsNullOrEmpty(dateBackOrder)) return new ServiceResponse<bool>(false, "Mã xuất kho không được để trống");
            string[] formats = {
                "M/d/yyyy h:mm:ss tt",
                "MM/dd/yyyy hh:mm:ss tt",
                "dd/MM/yyyy","dd/MM/yyyy hh:mm:ss tt"


            };
            if (!DateTime.TryParseExact(dateBackOrder, formats, new CultureInfo("vi-VN"), DateTimeStyles.None, out DateTime parsedDate))
            {
                return new ServiceResponse<bool>(false, "Ngày dựa kiến giao không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var purchaseOrderHeader = itemToBackOrder.First().PurchaseOrderCodeNavigation;

                    var backOrderCode = "BackOrder/" + decodedPurchaseOrderDetailCode;
                    var newPurchaseOrder = new PurchaseOrder
                    {
                        PurchaseOrderCode = backOrderCode,
                        OrderDate = DateTime.Now,
                        ExpectedDate = parsedDate,
                        WarehouseCode = purchaseOrderHeader.WarehouseCode,
                        SupplierCode = purchaseOrderHeader.SupplierCode,
                        StatusId = 1, // Trạng thái mới là "Đang chờ xử lý"
                        PurchaseOrderDescription = backOrderDescription,
                    };

                    await _purchaseOrderRepository.AddAsync(newPurchaseOrder, saveChanges: false);

                    foreach (var item in itemToBackOrder)
                    {
                        var quantityDiff = item.Quantity - item.QuantityReceived;
                        var backOrderDetail = new PurchaseOrderDetail
                        {
                            PurchaseOrderCode = backOrderCode,
                            ProductCode = item.ProductCode,
                            Quantity = quantityDiff, // Số lượng cần đặt hàng lại
                            QuantityReceived = 0 // Khởi tạo số lượng đã nhận là 0
                        };
                        item.Quantity = item.QuantityReceived; // Cập nhật số lượng đã nhận cho chi tiết đơn hàng gốc
                        await _purchaseOrderDetailRepository.AddAsync(backOrderDetail, saveChanges: false);
                        await _purchaseOrderDetailRepository.UpdateAsync(item, saveChanges: false);
                    }
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Tạo đơn hàng đặt lại thành công");

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
                    return new ServiceResponse<bool>(false, $"Lỗi không xác định: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeletePurchaseOrderDetailAsync(string purchaseOrderDetailCode, string productCode)
        {
            if (string.IsNullOrEmpty(purchaseOrderDetailCode) || string.IsNullOrEmpty(productCode))
            {
                return new ServiceResponse<bool>(false, "Mã chi tiết đơn hàng hoặc mã sản phẩm không được để trống.");
            }
            string decodedPurchaseOrderCode = Uri.UnescapeDataString(purchaseOrderDetailCode);
            var purchaseOrderDetail = await _purchaseOrderDetailRepository.GetPurchaseOrderDetailWithCode(decodedPurchaseOrderCode, productCode);
            if (purchaseOrderDetail == null)
            {
                return new ServiceResponse<bool>(false, "Chi tiết đơn hàng không tồn tại.");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    Expression<Func<PurchaseOrderDetail, bool>> expression = x => x.PurchaseOrderCode == decodedPurchaseOrderCode && x.ProductCode == productCode;
                    await _purchaseOrderDetailRepository.DeleteFirstByConditionAsync(expression);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa sản phẩm đặt hàng vì có dữ liệu tham chiếu.");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<PurchaseOrderDetailResponseDTO>>> GetAllPurchaseOrderDetails(string purchaseOrderCode, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(purchaseOrderCode) || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<PurchaseOrderDetailResponseDTO>>(false, "Dữ liệu đầu vào không hợp lệ.");
            }
            string decodedPurchaseOrderCode = Uri.UnescapeDataString(purchaseOrderCode);
            var purchaseOrderDetails = _purchaseOrderDetailRepository.GetAllPurchaseOrderDetailsAsync(decodedPurchaseOrderCode);

            var totalCount = await purchaseOrderDetails.CountAsync();
            var response = await purchaseOrderDetails
                .Select(x => new PurchaseOrderDetailResponseDTO
                {
                    PurchaseOrderCode = x.PurchaseOrderCode,
                    ProductCode = x.ProductCode,
                    ProductName = x.ProductCodeNavigation.ProductName!,
                    Quantity = x.Quantity,
                    QuantityReceived = x.QuantityReceived,
                })
                .OrderBy(x => x.ProductCode)
                .Take(pageSize)
                .Skip((page - 1) * pageSize)
                .ToListAsync(); ;
            var pagedResponse = new PagedResponse<PurchaseOrderDetailResponseDTO>(response, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<PurchaseOrderDetailResponseDTO>>(true, "Lấy danh sách chi tiết thành công ", pagedResponse);
        }

        public async Task<ServiceResponse<List<ProductMasterResponseDTO>>> GetListProductToPO()
        {
            var lstProduct = await _productMasterRepository.GetAllProduct(1, int.MaxValue);
            var lstProductForSI = lstProduct.Where(p => p.CategoryId != "SFG" && p.CategoryId != "FG")
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

        public async Task<ServiceResponse<PurchaseOrderDetailResponseDTO>> GetPurchaseOrderDetailByCode(string purchaseOrderDetailCode, string productCode)
        {
            if (string.IsNullOrEmpty(purchaseOrderDetailCode) || string.IsNullOrEmpty(productCode))
            {
                return new ServiceResponse<PurchaseOrderDetailResponseDTO>(false, "Mã chi tiết đơn hàng hoặc mã sản phẩm không được để trống.");
            }
            string decodedPurchaseOrderDetailCode = Uri.UnescapeDataString(purchaseOrderDetailCode);
            var purchaseOrderDetail = await _purchaseOrderDetailRepository.GetPurchaseOrderDetailWithCode(decodedPurchaseOrderDetailCode, productCode);
            if (purchaseOrderDetail == null)
            {
                return new ServiceResponse<PurchaseOrderDetailResponseDTO>(false, "Chi tiết đơn hàng không tồn tại.");
            }
            var response = new PurchaseOrderDetailResponseDTO
            {
                PurchaseOrderCode = purchaseOrderDetail.PurchaseOrderCode,
                ProductCode = purchaseOrderDetail.ProductCode,
                ProductName = purchaseOrderDetail.ProductCodeNavigation.ProductName!,
                Quantity = purchaseOrderDetail.Quantity,
                QuantityReceived = purchaseOrderDetail.QuantityReceived,
            };
            return new ServiceResponse<PurchaseOrderDetailResponseDTO>(true, "Lấy chi tiết đơn hàng thành công", response);
        }

        public async Task<ServiceResponse<bool>> UpdatePurchaseOrderDetail(PurchaseOrderDetailRequestDTO purchaseOrderDetail)
        {
            if (purchaseOrderDetail == null || string.IsNullOrEmpty(purchaseOrderDetail.PurchaseOrderCode) || string.IsNullOrEmpty(purchaseOrderDetail.ProductCode))
            {
                return new ServiceResponse<bool>(false, "Dữ liệu đầu vào không hợp lệ.");
            }

            var existingDetail = await _purchaseOrderDetailRepository.GetPurchaseOrderDetailWithCode(purchaseOrderDetail.PurchaseOrderCode, purchaseOrderDetail.ProductCode);
            if (existingDetail == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy chi tiết đơn hàng để cập nhật.");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingDetail.Quantity = purchaseOrderDetail.Quantity;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi không xác định: {ex.Message}");
                }
            }
        }
    }
}
