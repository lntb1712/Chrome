using Chrome.DTO;
using Chrome.DTO.ProductSupplierDTO;
using Chrome.Models;
using Chrome.Repositories.ProductSupplierRepository;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Chrome.Services.ProductSupplierSerivce
{
    public class ProductSupplierService:IProductSupplierService
    {
        private readonly IProductSupplierRepository _productSupplierRepository;
        private readonly ChromeContext _context;

        public ProductSupplierService(IProductSupplierRepository productSupplierRepository, ChromeContext context)
        {
            _productSupplierRepository = productSupplierRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddProductSupplier(ProductSupplierRequestDTO productSupplierRequestDTO)
        {
            if (productSupplierRequestDTO == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var productSupplier = new ProductSupplier
            {
                ProductCode = productSupplierRequestDTO.ProductCode,
                SupplierCode = productSupplierRequestDTO.SupplierCode,
                LeadTime = productSupplierRequestDTO.LeadTime,
                PricePerUnit = productSupplierRequestDTO.PricePerUnit,
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _productSupplierRepository.AddAsync(productSupplier, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm thông tin cung cấp thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if(dbEx.InnerException != null)
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi thêm sản phẩm: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteProductSupplier(string productCode, string supplierCode)
        {
            if(string.IsNullOrEmpty(productCode))
            {
                return new ServiceResponse<bool>(false, "Mã sản phẩm không được để trống");
            }
            var productSupplier = await _productSupplierRepository.GetProductSupplierWithProductCodeAndSupplierCode(productCode, supplierCode);
            if (productSupplier == null)
            {
                return new ServiceResponse<bool>(false, "Thông tin cung cấp không tồn tại");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    Expression<Func<ProductSupplier, bool>> predicate = p => p.ProductCode == productCode && p.SupplierCode == supplierCode;
                    await _productSupplierRepository.DeleteFirstByConditionAsync(predicate);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa thông tin cung cấp thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa thông tin cung cấp vì có dữ liệu tham chiếu.");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa thông tin cung cấp: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<ProductSupplierResponseDTO>>> GetAllProductSupplier(string productCode, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<ProductSupplierResponseDTO>>(false, "Trang và kích thước trang không hợp lệ");
            }

            var lstProductSupplier = await _productSupplierRepository.GetAllProductSupplierWithProductCode(productCode, page, pageSize);
            var totalProductSupplier = await _productSupplierRepository.GetTotalProductSupplierProductCodeCount(productCode);
            
            if(lstProductSupplier == null || lstProductSupplier.Count == 0)
            {
                return new ServiceResponse<PagedResponse<ProductSupplierResponseDTO>>(false, "Không có thông tin cung cấp");
            }

            var productSupplier = lstProductSupplier.Select(x => new ProductSupplierResponseDTO
            {
                ProductCode = x.ProductCode,
                ProductName = x.ProductCodeNavigation.ProductName!,
                SupplierCode = x.SupplierCode,
                SupplierName = x.SupplierCodeNavigation.SupplierName!,
                LeadTime = x.LeadTime,
                PricePerUnit = x.PricePerUnit,
            }).ToList();

            var pagedResponse = new PagedResponse<ProductSupplierResponseDTO>(productSupplier,page,pageSize,totalProductSupplier);
            return new ServiceResponse<PagedResponse<ProductSupplierResponseDTO>>(true, "Lấy thông tin cung cấp thành công", pagedResponse);
        }

        public async Task<ServiceResponse<bool>> UpdateProductSupplier(ProductSupplierRequestDTO productSupplierRequestDTO)
        {
            if (productSupplierRequestDTO == null || string.IsNullOrEmpty(productSupplierRequestDTO.ProductCode) || string.IsNullOrEmpty(productSupplierRequestDTO.SupplierCode))
            {
                return new ServiceResponse<bool>(false, "Thông tin cung cấp không hợp lệ");
            }

            var existingProductSupplier = await _productSupplierRepository.GetProductSupplierWithProductCodeAndSupplierCode(productSupplierRequestDTO.ProductCode, productSupplierRequestDTO.SupplierCode);
            if (existingProductSupplier == null)
            {
                return new ServiceResponse<bool>(false, "Thông tin cung cấp không tồn tại");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingProductSupplier.SupplierCode = productSupplierRequestDTO.SupplierCode;
                    existingProductSupplier.LeadTime = productSupplierRequestDTO.LeadTime;
                    existingProductSupplier.PricePerUnit = productSupplierRequestDTO.PricePerUnit;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật thông tin cung cấp");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật thông tin cung cấp: {ex.Message}");
                }
            }
        }
    }
}
