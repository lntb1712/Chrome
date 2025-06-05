using Chrome.DTO;
using Chrome.DTO.ProductMasterDTO;
using Chrome.Models;
using Chrome.Repositories.ProductMasterRepository;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Services.ProductMasterService
{
    public class ProductMasterService : IProductMasterService
    {
        private readonly IProductMasterRepository _productMasterRepository;
        private readonly ChromeContext _context;

        public ProductMasterService(IProductMasterRepository productMasterRepository, ChromeContext context)
        {
            _productMasterRepository = productMasterRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddProductMaster(ProductMasterRequestDTO product)
        {
            if (product == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ.");
            }
            var productMaster = new ProductMaster
            {
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductImage = product.ProductImage,
                CategoryId = product.CategoryId,
                BaseQuantity = product.BaseQuantity,
                Uom = product.Uom,
                BaseUom = product.BaseUom,
                Valuation = product.Valuation,
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _productMasterRepository.AddAsync(productMaster,saveChanges:false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm sản phẩm thành công.");
                }
                catch(DbUpdateException dbEx)
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi thêm sản phẩm: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteProductMaster(string productCode)
        {
            if(string.IsNullOrEmpty(productCode))
            {
                return new ServiceResponse<bool>(false, "Mã sản phẩm không được để trống.");
            }
            var product = await _productMasterRepository.GetProductMasterWithProductCode(productCode);
            if (product == null)
            {
                return new ServiceResponse<bool>(false, "Sản phẩm không tồn tại.");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _productMasterRepository.DeleteAsync(productCode, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa sản phẩm thành công.");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa sản phẩm vì có dữ liệu tham chiếu.");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa sản phẩm: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<ProductMasterResponseDTO>>> GetAllProductMaster(int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<ProductMasterResponseDTO>>(false, "Trang và kích thước trang phải lớn hơn 0.");
            }
            var lstProduct = await _productMasterRepository.GetAllProduct(page, pageSize);
            var totalCount = await _productMasterRepository.GetTotalProductCount();
            if(lstProduct == null || lstProduct.Count ==0)
            {
                return new ServiceResponse<PagedResponse<ProductMasterResponseDTO>>(false, "Không có sản phẩm nào.");
            }
            var productResponse = lstProduct.Select(p => new ProductMasterResponseDTO
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
                Valuation= (float?)p.Valuation,
                TotalOnHand = (float)(p.Inventories.Where(t => t.ProductCode == p.ProductCode).Sum(i => i.Quantity) ?? 0.00), // Assuming Quantity is a property in Inventory
            }).ToList();

            var pagedResponse = new PagedResponse<ProductMasterResponseDTO>(productResponse, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<ProductMasterResponseDTO>>(true, "Lấy danh sách sản phẩm thành công.",pagedResponse);
        }

        public async Task<ServiceResponse<PagedResponse<ProductMasterResponseDTO>>> GetAllProductWithCategoryID(string categoryId, int page, int pageSize)
        {
            if(string.IsNullOrEmpty(categoryId) || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<ProductMasterResponseDTO>>(false, "Danh mục hoặc trang không hợp lệ.");
            }
            var lstProduct = await _productMasterRepository.GetAllProductWithCategoryID(categoryId, page, pageSize);
            var totalCount = await _productMasterRepository.GetTotalProductWithCategoryIDCount(categoryId);
            if (lstProduct == null || lstProduct.Count == 0)
            {
                return new ServiceResponse<PagedResponse<ProductMasterResponseDTO>>(false, "Không có sản phẩm nào trong danh mục này.");
            }
            var productResponse = lstProduct.Select(p => new ProductMasterResponseDTO
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
                TotalOnHand = (float)(p.Inventories.Where(t => t.ProductCode == p.ProductCode).Sum(i => i.Quantity) ?? 0.00), // Assuming Quantity is a property in Inventory
            }).ToList();
            var pagedResponse = new PagedResponse<ProductMasterResponseDTO>(productResponse, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<ProductMasterResponseDTO>>(true, "Lấy danh sách sản phẩm theo danh mục thành công.", pagedResponse);
        }

        public async Task<ServiceResponse<ProductMasterResponseDTO>> GetProductMasterWithProductCode(string productCode)
        {
            if(string.IsNullOrEmpty(productCode))
            {
                return new ServiceResponse<ProductMasterResponseDTO>(false, "Mã sản phẩm không được để trống.");
            }
            var product = await _productMasterRepository.GetProductMasterWithProductCode(productCode);
            if (product == null)
            {
                return new ServiceResponse<ProductMasterResponseDTO>(false, "Sản phẩm không tồn tại.");
            }
            var productResponse = new ProductMasterResponseDTO
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
                TotalOnHand = (float)(product.Inventories.Where(t => t.ProductCode == product.ProductCode).Sum(i => i.Quantity) ?? 0.00),
            };
            return new ServiceResponse<ProductMasterResponseDTO>(true, "Lấy thông tin sản phẩm thành công.", productResponse);
        }

        public async Task<ServiceResponse<int>> GetTotalProductCount()
        {
            try
            {
                var totalCount = await _productMasterRepository.GetTotalProductCount();
                return new ServiceResponse<int>(true, "Lấy tổng số sản phẩm thành công.", totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, $"Lỗi khi lấy tổng số sản phẩm: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<ProductMasterResponseDTO>>> SearchProduct(string textToSearch, int page, int pageSize)
        {
            if(string.IsNullOrEmpty(textToSearch) || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<ProductMasterResponseDTO>>(false, "Từ khóa tìm kiếm hoặc trang không hợp lệ.");
            }
            var lstProduct = await _productMasterRepository.SearchProduct(textToSearch, page, pageSize);
            var totalCount = await _productMasterRepository.GetTotalSearchCount(textToSearch);
            if (lstProduct == null || lstProduct.Count == 0)
            {
                return new ServiceResponse<PagedResponse<ProductMasterResponseDTO>>(false, "Không tìm thấy sản phẩm nào.");
            }
            var productResponse = lstProduct.Select(p => new ProductMasterResponseDTO
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
                TotalOnHand = (float)(p.Inventories.Where(t => t.ProductCode == p.ProductCode).Sum(i => i.Quantity) ?? 0.00), // Assuming Quantity is a property in Inventory
            }).ToList();
            var pagedResponse = new PagedResponse<ProductMasterResponseDTO>(productResponse, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<ProductMasterResponseDTO>>(true, "Tìm kiếm sản phẩm thành công.", pagedResponse);
        }

        public async Task<ServiceResponse<bool>> UpdateProductMaster(ProductMasterRequestDTO product)
        {
            if (product == null || string.IsNullOrEmpty(product.ProductCode))
            {
                return new ServiceResponse<bool>(false, "Dữ liệu sản phẩm không hợp lệ.");
            }
            var existingProduct = await _productMasterRepository.GetProductMasterWithProductCode(product.ProductCode);
            if (existingProduct == null)
            {
                return new ServiceResponse<bool>(false, "Sản phẩm không tồn tại.");
            }
            using(var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingProduct.ProductName = product.ProductName;
                    existingProduct.ProductDescription = product.ProductDescription;
                    existingProduct.ProductImage = product.ProductImage;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.BaseQuantity = product.BaseQuantity;
                    existingProduct.Uom = product.Uom;
                    existingProduct.BaseUom = product.BaseUom;
                    existingProduct.Valuation =product.Valuation;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật sản phẩm thành công.");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật sản phẩm: {ex.Message}");
                }
            }
        }
    }
}
