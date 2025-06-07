using Chrome.DTO;
using Chrome.DTO.ProductCustomerDTO;
using Chrome.Models;
using Chrome.Repositories.ProductCustomerRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Chrome.Services.ProductCustomerService
{
    public class ProductCustomerService: IProductCustomerService
    {
        private readonly IProductCustomerRepository _productCustomerRepository;
        private readonly ChromeContext _context;

        public ProductCustomerService(IProductCustomerRepository productCustomerRepository, ChromeContext context)
        {
            _productCustomerRepository = productCustomerRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddProductCustomer(ProductCustomerRequestDTO productCustomer)
        {
            if(productCustomer == null)
            {
                return new ServiceResponse<bool> (false,"Dữ liệu không hợp lệ");    
            }    
            
            if(string.IsNullOrEmpty(productCustomer.ProductCode) || string.IsNullOrEmpty(productCustomer.CustomerCode))
            {
                return new ServiceResponse<bool>(false, "Mã sản phẩm hoặc mã khách hàng không được để trống");
            }

            var productCustomerEntity = new CustomerProduct
            {
                ProductCode = productCustomer.ProductCode,
                CustomerCode = productCustomer.CustomerCode,
                ExpectedDeliverTime = productCustomer.ExpectedDeliverTime,
                PricePerUnit = productCustomer.PricePerUnit
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _productCustomerRepository.AddAsync(productCustomerEntity,saveChanges:false);
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi thêm sản phẩm khách hàng: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteProductCustomer(string productCode, string customerCode)
        {
            if(string.IsNullOrEmpty(productCode)|| string.IsNullOrEmpty(customerCode))
            {
                return new ServiceResponse<bool>(false, "Mã sản phẩm hoặc mã khách hàng không được để trống");
            }
            productCode = productCode.Trim();
            customerCode = customerCode.Trim();
            var productCustomer = await _productCustomerRepository.GetAllCustomerProductsByCode(productCode, customerCode);
            if (productCustomer == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy sản phẩm khách hàng với mã sản phẩm và mã khách hàng đã cho");
            }
            using(var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    Expression<Func<CustomerProduct, bool>> predicate = x => x.ProductCode == productCode && x.CustomerCode == customerCode;
                    await _productCustomerRepository.DeleteFirstByConditionAsync(predicate, saveChanges: false);
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
                            return new ServiceResponse<bool>(false, "Không thể xóa do dữ liệu tham chiếu");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa sản phẩm khách hàng: {ex.Message}");
                }
            }

        }

        public async Task<ServiceResponse<PagedResponse<ProductCustomerResponseDTO>>> GetAllProductCustomer(string productCode, int page, int pageSize)
        {
            if(string.IsNullOrEmpty(productCode))
            {
                return new ServiceResponse<PagedResponse<ProductCustomerResponseDTO>>(false, "Mã sản phẩm không được để trống");
            }
            if(page<1|| pageSize<1)
            {
                return new ServiceResponse<PagedResponse<ProductCustomerResponseDTO>>(false, "Trang và kích thước trang phải lớn hơn 0");
            }

            productCode = productCode.Trim();
            var productCustomers = await _productCustomerRepository.GetAllCustomerProducts(productCode, page, pageSize);
            if (productCustomers == null || !productCustomers.Any())
            {
                return new ServiceResponse<PagedResponse<ProductCustomerResponseDTO>>(false, "Không tìm thấy sản phẩm khách hàng với mã sản phẩm đã cho");
            }
            var totalCount = await _productCustomerRepository.GetTotalCustomerProductCount(productCode);
            var lstProductCustomer = productCustomers.Select(pc => new ProductCustomerResponseDTO
            {
                ProductCode = pc.ProductCode,
                CustomerCode = pc.CustomerCode,
                ExpectedDeliverTime = pc.ExpectedDeliverTime,
                PricePerUnit = pc.PricePerUnit
            }).ToList();
            var pagedResponse = new PagedResponse<ProductCustomerResponseDTO>(lstProductCustomer, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<ProductCustomerResponseDTO>>(true, "Lấy danh sách sản phẩm khách hàng thành công",pagedResponse);

        }

        public async Task<ServiceResponse<bool>> UpdateProductCustomer(ProductCustomerRequestDTO productCustomer)
        {
            if(productCustomer == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu không hợp lệ");
            }
            if(string.IsNullOrEmpty(productCustomer.ProductCode) || string.IsNullOrEmpty(productCustomer.CustomerCode))
            {
                return new ServiceResponse<bool>(false, "Mã sản phẩm hoặc mã khách hàng không được để trống");
            }
            productCustomer.ProductCode = productCustomer.ProductCode.Trim();
            productCustomer.CustomerCode = productCustomer.CustomerCode.Trim();
            var existingProductCustomer = await _productCustomerRepository.GetAllCustomerProductsByCode(productCustomer.ProductCode, productCustomer.CustomerCode);
            if(existingProductCustomer == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy sản phẩm khách hàng với mã sản phẩm và mã khách hàng đã cho");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingProductCustomer.CustomerCode = productCustomer.CustomerCode;
                    existingProductCustomer.ExpectedDeliverTime = productCustomer.ExpectedDeliverTime;
                    existingProductCustomer.PricePerUnit = productCustomer.PricePerUnit;
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật sản phẩm khách hàng: {ex.Message}");
                }
            }
        }
    }
}
