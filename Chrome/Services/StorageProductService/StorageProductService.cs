using Chrome.DTO;
using Chrome.DTO.StorageProductDTO;
using Chrome.Models;
using Chrome.Repositories.StorageProductRepository;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Services.StorageProductService
{
    public class StorageProductService : IStorageProductService
    {
        private readonly IStorageProductRepository _storageProductRepository;
        private readonly ChromeContext _context;

        public StorageProductService(IStorageProductRepository storageProductRepository, ChromeContext context)
        {
            _storageProductRepository = storageProductRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddStorageProduct(StorageProductRequestDTO storageProductRequestDTO)
        {
            if(storageProductRequestDTO == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }    
            var storageProduct = new StorageProduct
            {
                StorageProductId = storageProductRequestDTO.StorageProductId,
                StorageProductName = storageProductRequestDTO.StorageProductName,
                ProductCode = storageProductRequestDTO.ProductCode,
                MaxQuantity = storageProductRequestDTO.MaxQuantity
            };

            using(var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _storageProductRepository.AddAsync(storageProduct,saveChanges:false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm sản phẩm kho thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi thêm sản phẩm kho: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteStorageProduct(string storageProductCode)
        {
            if(string.IsNullOrEmpty(storageProductCode))
            {
                return new ServiceResponse<bool>(false, "Mã sản phẩm kho không hợp lệ");
            }    
            var storageProduct = await _storageProductRepository.GetStorageProductWithCode(storageProductCode);
            if(storageProduct == null)
            {
                return new ServiceResponse<bool>(false, "Sản phẩm kho không tồn tại");
            }
            using(var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _storageProductRepository.DeleteAsync(storageProductCode, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa sản phẩm kho thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa sản phẩm kho vì có dữ liệu tham chiếu");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa sản phẩm kho: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<StorageProductResponseDTO>>> GetAllStorageProducts(int page, int pageSize)
        {
            if(page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<StorageProductResponseDTO>>(false, "Trang hoặc kích thước trang không hợp lệ");
            }
            var storageProducts = await _storageProductRepository.GetAllStorageProducts(page, pageSize);
            if(storageProducts == null || !storageProducts.Any())
            {
                return new ServiceResponse<PagedResponse<StorageProductResponseDTO>>(false, "Không có sản phẩm kho nào");
            }
            var totalCount = await _storageProductRepository.GetTotalStorageProductCount();
            var storageProductResponses = storageProducts.Select(sp => new StorageProductResponseDTO
            {
                StorageProductId = sp.StorageProductId,
                StorageProductName = sp.StorageProductName,
                ProductCode = sp.ProductCode,
                ProductName = sp.ProductCodeNavigation?.ProductName,
                MaxQuantity = sp.MaxQuantity,
               
            }).ToList();
            var pagedResponse = new PagedResponse<StorageProductResponseDTO>(storageProductResponses, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<StorageProductResponseDTO>>(true, "Lấy danh sách sản phẩm kho thành công",pagedResponse);
        }

        public async Task<ServiceResponse<StorageProductResponseDTO>> GetStorageProductWithCode(string storageProductCode)
        {
            if(string.IsNullOrEmpty(storageProductCode))
            {
                return new ServiceResponse<StorageProductResponseDTO>(false, "Mã sản phẩm kho không hợp lệ");
            }
            var storageProduct = await _storageProductRepository.GetStorageProductWithCode(storageProductCode);
            if(storageProduct == null)
            {
                return new ServiceResponse<StorageProductResponseDTO>(false, "Sản phẩm kho không tồn tại");
            }
            var storageProductResponse = new StorageProductResponseDTO
            {
                StorageProductId = storageProduct.StorageProductId,
                StorageProductName = storageProduct.StorageProductName,
                ProductCode = storageProduct.ProductCode,
                ProductName = storageProduct.ProductCodeNavigation?.ProductName,
                MaxQuantity = storageProduct.MaxQuantity
            };
            return new ServiceResponse<StorageProductResponseDTO>(true, "Lấy sản phẩm kho thành công",storageProductResponse);
        }

        public async Task<ServiceResponse<int>> GetTotalStorageProductCount()
        {
            try
            {
                var totalCount = await _storageProductRepository.GetTotalStorageProductCount();
                return new ServiceResponse<int>(true, "Lấy tổng số sản phẩm kho thành công", totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, $"Lỗi khi lấy tổng số sản phẩm kho: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<StorageProductResponseDTO>>> SearchStorageProducts(string textToSearch, int page, int pageSize)
        {
            if(page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<StorageProductResponseDTO>>(false, "Trang hoặc kích thước trang không hợp lệ");
            }
            if(string.IsNullOrEmpty(textToSearch))
            {
                return new ServiceResponse<PagedResponse<StorageProductResponseDTO>>(false, "Chuỗi tìm kiếm không hợp lệ");
            }
            var storageProducts = await _storageProductRepository.SearchStorageProducts(textToSearch, page, pageSize);
            if(storageProducts == null || !storageProducts.Any())
            {
                return new ServiceResponse<PagedResponse<StorageProductResponseDTO>>(false, "Không có sản phẩm kho nào phù hợp với tìm kiếm");
            }
            var totalCount = await _storageProductRepository.GetTotalSearchCount(textToSearch);
            var storageProductResponses = storageProducts.Select(sp => new StorageProductResponseDTO
            {
                StorageProductId = sp.StorageProductId,
                StorageProductName = sp.StorageProductName,
                ProductCode = sp.ProductCode,
                ProductName = sp.ProductCodeNavigation?.ProductName,
                MaxQuantity = sp.MaxQuantity
            }).ToList();
            var pagedResponse = new PagedResponse<StorageProductResponseDTO>(storageProductResponses, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<StorageProductResponseDTO>>(true, "Tìm kiếm sản phẩm kho thành công",pagedResponse);
        }

        public async Task<ServiceResponse<bool>> UpdateStorageProduct(StorageProductRequestDTO storageProductRequestDTO)
        {
            if(storageProductRequestDTO == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var existingStorageProduct = await _storageProductRepository.GetStorageProductWithCode(storageProductRequestDTO.StorageProductId);
            if(existingStorageProduct == null)
            {
                return new ServiceResponse<bool>(false, "Sản phẩm kho không tồn tại");
            }
            using( var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingStorageProduct.StorageProductName = storageProductRequestDTO.StorageProductName;
                    existingStorageProduct.ProductCode = storageProductRequestDTO.ProductCode;
                    existingStorageProduct.MaxQuantity = storageProductRequestDTO.MaxQuantity;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật sản phẩm kho thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật sản phẩm kho: {ex.Message}");
                }
            }
        }
    }
}
