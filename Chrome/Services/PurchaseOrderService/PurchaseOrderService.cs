using Chrome.DTO;
using Chrome.DTO.PurchaseOrderDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.DTO.SupplierMasterDTO;
using Chrome.DTO.WarehouseMasterDTO;
using Chrome.Models;
using Chrome.Repositories.PurchaseOrderRepository;
using Chrome.Repositories.StatusMasterRepository;
using Chrome.Repositories.SupplierMasterRepository;
using Chrome.Repositories.WarehouseMasterRepository;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Chrome.Services.PurchaseOrderService
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly ISupplierMasterRepository _supplierMasterRepository;
        private readonly IStatusMasterRepository _statusMasterRepository;
        private readonly IWarehouseMasterRepository _warehouseMasterRepository;
        private readonly ChromeContext _context;

        public PurchaseOrderService(IPurchaseOrderRepository purchaseOrderRepository, ISupplierMasterRepository supplierMasterRepository, IStatusMasterRepository statusMasterRepository, IWarehouseMasterRepository warehouseMasterRepository, ChromeContext context)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
            _supplierMasterRepository = supplierMasterRepository;
            _statusMasterRepository = statusMasterRepository;
            _warehouseMasterRepository = warehouseMasterRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddPurchaseOrder(PurchaseOrderRequestDTO purchaseOrder)
        {
            if (purchaseOrder == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            string[] formats = {
                "M/d/yyyy h:mm:ss tt",
                "MM/dd/yyyy hh:mm:ss tt",
                "dd/MM/yyyy"
            };
            if (!DateTime.TryParseExact(purchaseOrder.OrderDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedOrderDate))
            {
                return new ServiceResponse<bool>(false, "Ngày đặt hàng không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }
            if (!DateTime.TryParseExact(purchaseOrder.OrderDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedExpectedDate))
            {
                return new ServiceResponse<bool>(false, "Ngày nhận hàng dự kiến không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }
            var purchaseOrderEntity = new PurchaseOrder
            {
                PurchaseOrderCode = purchaseOrder.PurchaseOrderCode,
                WarehouseCode = purchaseOrder.WarehouseCode,
                StatusId = 1,
                OrderDate = parsedOrderDate,
                ExpectedDate = parsedExpectedDate,
                SupplierCode = purchaseOrder.SupplierCode,
                PurchaseOrderDescription = purchaseOrder.PurchaseOrderDescription
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Thêm đơn hàng mua vào cơ sở dữ liệu
                    await _purchaseOrderRepository.AddAsync(purchaseOrderEntity, saveChanges: false);
                    // Lưu thay đổi
                    await _context.SaveChangesAsync();
                    // Commit transaction
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm đơn hàng mua thành công");
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
                    // Rollback transaction nếu có lỗi xảy ra
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi thêm đơn hàng mua: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeletePurchaseOrderAsync(string purchaseOrderCode)
        {
            if (string.IsNullOrEmpty(purchaseOrderCode))
            {
                return new ServiceResponse<bool>(false, "Mã đơn hàng mua không hợp lệ");
            }

            var purchaseOrder = await _purchaseOrderRepository.GetPurchaseOrderWithCode(purchaseOrderCode);
            if (purchaseOrder == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy đơn hàng mua với mã đã cho");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Xóa đơn hàng mua
                    await _purchaseOrderRepository.DeleteAsync(purchaseOrderCode, saveChanges: false);
                    // Lưu thay đổi
                    await _context.SaveChangesAsync();
                    // Commit transaction
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa đơn hàng mua thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa đơn hàng mua vì có dữ liệu tham chiếu");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    // Rollback transaction nếu có lỗi xảy ra
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa đơn hàng mua: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<PurchaseOrderResponseDTO>>> GetAllPurchaseOrders(string[] warehouseCodes, int page, int pageSize)
        {
            if (warehouseCodes == null || warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<PurchaseOrderResponseDTO>>(false, "Mã kho không hợp lệ");
            }
            var query = _purchaseOrderRepository.GetAllPurchaseOrdersAsync(warehouseCodes);
            var totalCount = await query.CountAsync();
            var purchaseOrders = await query
                .Select(x => new PurchaseOrderResponseDTO
                {
                    PurchaseOrderCode = x.PurchaseOrderCode,
                    WarehouseCode = x.WarehouseCode,
                    StatusId = x.StatusId,
                    WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                    OrderDate = x.OrderDate!.Value.ToString("dd/MM/yyyy"),
                    ExpectedDate = x.ExpectedDate!.Value.ToString("dd/MM/yyyy"),
                    SupplierCode = x.SupplierCode,
                    PurchaseOrderDescription = x.PurchaseOrderDescription,
                    StatusName = x.Status!.StatusName,
                    SupplierName = x.SupplierCodeNavigation!.SupplierName
                })
                .OrderBy(x => x.StatusId)
                .Take(pageSize)
                .Skip((page - 1) * pageSize)
                .ToListAsync();

            var pagedResponse = new PagedResponse<PurchaseOrderResponseDTO>(purchaseOrders, page, pageSize, totalCount);
            
            return new ServiceResponse<PagedResponse<PurchaseOrderResponseDTO>>(true,"Lấy ng sách đơn hàng mua thành công", pagedResponse);

        }

        public async Task<ServiceResponse<PagedResponse<PurchaseOrderResponseDTO>>> GetAllPurchaseOrdersWithStatus(string[] warehouseCodes, int statusId, int page, int pageSize)
        {
            if (warehouseCodes == null ||statusId <0|| warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<PurchaseOrderResponseDTO>>(false, "Mã kho không hợp lệ");
            }
            var query = _purchaseOrderRepository.GetAllPurchaseOrderWithStatus(warehouseCodes,statusId);
            var totalCount = await query.CountAsync();
            var purchaseOrders = await query
                .Select(x => new PurchaseOrderResponseDTO
                {
                    PurchaseOrderCode = x.PurchaseOrderCode,
                    WarehouseCode = x.WarehouseCode,
                    StatusId = x.StatusId,
                    WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                    OrderDate = x.OrderDate!.Value.ToString("dd/MM/yyyy"),
                    ExpectedDate = x.ExpectedDate!.Value.ToString("dd/MM/yyyy"),
                    SupplierCode = x.SupplierCode,
                    PurchaseOrderDescription = x.PurchaseOrderDescription,
                    StatusName = x.Status!.StatusName,
                    SupplierName = x.SupplierCodeNavigation!.SupplierName
                })
                .OrderBy(x => x.StatusId)
                .Take(pageSize)
                .Skip((page - 1) * pageSize)
                .ToListAsync();

            var pagedResponse = new PagedResponse<PurchaseOrderResponseDTO>(purchaseOrders, page, pageSize, totalCount);

            return new ServiceResponse<PagedResponse<PurchaseOrderResponseDTO>>(true, "Lấy ng sách đơn hàng mua thành công", pagedResponse);
        }

        public async  Task<ServiceResponse<List<StatusMasterResponseDTO>>> GetListStatusMaster()
        {
            var lstStatus = await _statusMasterRepository.GetAllStatuses();
            var lstStatusResponse = lstStatus.Select(x => new StatusMasterResponseDTO
            {
                StatusId = x.StatusId,
                StatusName = x.StatusName,
            }).ToList();
            return new ServiceResponse<List<StatusMasterResponseDTO>>(true, "Lấy danh sách trạng thái thành công", lstStatusResponse);
        }

        public async Task<ServiceResponse<List<SupplierMasterResponseDTO>>> GetListSupplierMasterAsync()
        {
            var lstSupplier = await _supplierMasterRepository.GetAllSupplier(1, int.MaxValue);
            var lstSupplierResponse = lstSupplier.Select(x => new SupplierMasterResponseDTO
            {
                SupplierCode = x.SupplierCode,
                SupplierName = x.SupplierName,
                SupplierAddress = x.SupplierAddress,
                SupplierPhone = x.SupplierPhone,
            }).ToList();
            return new ServiceResponse<List<SupplierMasterResponseDTO>>(true, "Lấy danh sách nhà cung cấp thành công", lstSupplierResponse);
        }
        
        public async Task<ServiceResponse<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission(string[] warehouseCodes)
        {
            if (warehouseCodes.Length == 0)
            {
                return new ServiceResponse<List<WarehouseMasterResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var response = await _warehouseMasterRepository.GetWarehouseMasters(1, int.MaxValue);
            var lstWarehouseMapping = response.Where(x => warehouseCodes.Contains(x.WarehouseCode))
                                              .Select(x => new WarehouseMasterResponseDTO
                                              {
                                                  WarehouseCode = x.WarehouseCode,
                                                  WarehouseName = x.WarehouseName,
                                                  WarehouseAddress = x.WarehouseAddress,
                                                  WarehouseDescription = x.WarehouseDescription,
                                              })
                                              .ToList();
            return new ServiceResponse<List<WarehouseMasterResponseDTO>>(true, "Lấy danh sách kho dựa theo quyền thành công", lstWarehouseMapping);
        }

        public async Task<ServiceResponse<PurchaseOrderResponseDTO>> GetPurchaseOrderByCode(string purchaseOrderCode)
        {
            if(string.IsNullOrEmpty(purchaseOrderCode))
            {
                return new ServiceResponse<PurchaseOrderResponseDTO>(false, "Mã đơn hàng mua không hợp lệ");
            }

            var purchaseOrder = await _purchaseOrderRepository.GetPurchaseOrderWithCode(purchaseOrderCode);
            if (purchaseOrder == null)
            {
                return new ServiceResponse<PurchaseOrderResponseDTO>(false, "Đơn hàng mua không tồn tại");
            }
            
            return new ServiceResponse<PurchaseOrderResponseDTO>(true, "Lấy đơn hàng mua thành công", new PurchaseOrderResponseDTO
            {
                PurchaseOrderCode = purchaseOrder.PurchaseOrderCode,
                WarehouseCode = purchaseOrder.WarehouseCode,
                StatusId = purchaseOrder.StatusId,
                OrderDate = purchaseOrder.OrderDate?.ToString("dd/MM/yyyy"),
                ExpectedDate = purchaseOrder.ExpectedDate?.ToString("dd/MM/yyyy"),
                SupplierCode = purchaseOrder.SupplierCode,
                PurchaseOrderDescription = purchaseOrder.PurchaseOrderDescription,
                StatusName = purchaseOrder.Status!.StatusName,
                SupplierName = purchaseOrder.SupplierCodeNavigation!.SupplierName
            });


        }

        public async Task<ServiceResponse<PagedResponse<PurchaseOrderResponseDTO>>> SearchPurchaseOrderAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize)
        {
            if( warehouseCodes == null || string.IsNullOrEmpty(textToSearch) || warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<PurchaseOrderResponseDTO>>(false, "Mã kho hoặc từ khóa tìm kiếm không hợp lệ");
            }
            var query = _purchaseOrderRepository.SearchPurchaseOrderAsync(warehouseCodes, textToSearch);
            var totalCount = await query.CountAsync();
            var purchaseOrders = await query
                .Select(x => new PurchaseOrderResponseDTO
                {
                    PurchaseOrderCode = x.PurchaseOrderCode,
                    WarehouseCode = x.WarehouseCode,
                    StatusId = x.StatusId,
                    WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                    OrderDate = x.OrderDate!.Value.ToString("dd/MM/yyyy"),
                    ExpectedDate = x.ExpectedDate!.Value.ToString("dd/MM/yyyy"),
                    SupplierCode = x.SupplierCode,
                    PurchaseOrderDescription = x.PurchaseOrderDescription,
                    StatusName = x.Status!.StatusName,
                    SupplierName = x.SupplierCodeNavigation!.SupplierName
                })
                .OrderBy(x => x.StatusId)
                .Take(pageSize)
                .Skip((page - 1) * pageSize)
                .ToListAsync();
            var pagedResponse = new PagedResponse<PurchaseOrderResponseDTO>(purchaseOrders, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<PurchaseOrderResponseDTO>>(true, "Tìm kiếm đơn hàng mua thành công", pagedResponse);
        }

        public async Task<ServiceResponse<bool>> UpdatePurchaseOrder(PurchaseOrderRequestDTO purchaseOrder)
        {
            if(purchaseOrder==null) return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");

            var existingOrder = await _purchaseOrderRepository.GetPurchaseOrderWithCode(purchaseOrder.PurchaseOrderCode);
            if (existingOrder == null)
            {
                return new ServiceResponse<bool>(false, "Đơn hàng mua không tồn tại");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    string[] formats = {
                        "M/d/yyyy h:mm:ss tt",
                        "MM/dd/yyyy hh:mm:ss tt",
                        "dd/MM/yyyy"
                    };
                    if (!DateTime.TryParseExact(purchaseOrder.OrderDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedOrderDate))
                    {
                        return new ServiceResponse<bool>(false, "Ngày đặt hàng không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
                    }
                    if (!DateTime.TryParseExact(purchaseOrder.ExpectedDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedExpectedDate))
                    {
                        return new ServiceResponse<bool>(false, "Ngày nhận hàng dự kiến không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
                    }
                    existingOrder.WarehouseCode = purchaseOrder.WarehouseCode;
                    existingOrder.StatusId = purchaseOrder.StatusId;
                    existingOrder.OrderDate = parsedOrderDate;
                    existingOrder.ExpectedDate = parsedExpectedDate;
                    existingOrder.SupplierCode = purchaseOrder.SupplierCode;
                    existingOrder.PurchaseOrderDescription = purchaseOrder.PurchaseOrderDescription;
                    // Cập nhật đơn hàng mua
                    await _purchaseOrderRepository.UpdateAsync(existingOrder, saveChanges: false);

                    // Lưu thay đổi
                    await _context.SaveChangesAsync();

                    // Commit transaction
                    await transaction.CommitAsync();

                    return new ServiceResponse<bool>(true, "Cập nhật đơn hàng mua thành công");
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
                    // Rollback transaction nếu có lỗi xảy ra
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật đơn hàng mua: {ex.Message}");
                }
            }
        }
    }
}
