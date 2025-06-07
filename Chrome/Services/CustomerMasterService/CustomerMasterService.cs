using Chrome.DTO;
using Chrome.DTO.CustomerMasterDTO;
using Chrome.Models;
using Chrome.Repositories.CustomerMasterRepository;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Services.CustomerMasterService
{
    public class CustomerMasterService : ICustomerMasterService
    {
        private readonly ICustomerMasterRepository _customerMasterRepository;
        private readonly ChromeContext _context;

        public CustomerMasterService(ICustomerMasterRepository customerMasterRepository, ChromeContext context)
        {
            _customerMasterRepository = customerMasterRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddCustomerMaster(CustomerMasterRequestDTO customer)
        {
            if (customer == null)
            {
                return new ServiceResponse<bool>(false, "Du liệu không hợp lệ");
            }

            var customerMaster = new CustomerMaster
            {
                CustomerCode = customer.CustomerCode,
                CustomerName = customer.CustomerName,
                CustomerAddress = customer.CustomerAddress,
                CustomerPhone = customer.CustomerPhone,
                CustomerEmail = customer.CustomerEmail,
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _customerMasterRepository.AddAsync(customerMaster, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm khách hàng thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi thêm khách hàng: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteCustomerMaster(string customerCode)
        {
            if (string.IsNullOrEmpty(customerCode))
            {
                return new ServiceResponse<bool>(false, "Mã khách hàng không hợp lệ");
            }
            customerCode = customerCode.Trim();
            var customer = await _customerMasterRepository.GetCustomerMasterWithCustomerCode(customerCode);
            if (customer == null)
            {
                return new ServiceResponse<bool>(false, "Khách hàng không tồn tại");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {

                    await _customerMasterRepository.DeleteAsync(customerCode, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa khách hàng thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa khách hàng vì có dữ liệu tham chiếu");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa khách hàng: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<CustomerMasterResponseDTO>>> GetAllCustomerMaster(int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<CustomerMasterResponseDTO>>(false, "Trang hoặc kích thước trang không hợp lệ");
            }
            var customers = await _customerMasterRepository.GetAllCustomer(page, pageSize);
            var totalCount = await _customerMasterRepository.GetTotalCustomerCount();
            if (customers == null || customers.Count == 0)
            {
                return new ServiceResponse<PagedResponse<CustomerMasterResponseDTO>>(false, "Không có khách hàng nào");
            }
            var lstCustomerResponse = customers.Select(c => new CustomerMasterResponseDTO
            {
                CustomerCode = c.CustomerCode,
                CustomerName = c.CustomerName,
                CustomerAddress = c.CustomerAddress,
                CustomerPhone = c.CustomerPhone,
                CustomerEmail = c.CustomerEmail
            }).ToList();
            var pagedResponse = new PagedResponse<CustomerMasterResponseDTO>(lstCustomerResponse, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<CustomerMasterResponseDTO>>(true, "Lấy danh sách khách hàng thành công", pagedResponse);
        }

        public async Task<ServiceResponse<CustomerMasterResponseDTO>> GetCustomerWithCustomerCode(string customerCode)
        {
            if (string.IsNullOrEmpty(customerCode))
            {
                return new ServiceResponse<CustomerMasterResponseDTO>(false, "Mã khách hàng không hợp lệ");
            }
            customerCode = customerCode.Trim();
            var customer = await _customerMasterRepository.GetCustomerMasterWithCustomerCode(customerCode);
            if (customer == null)
            {
                return new ServiceResponse<CustomerMasterResponseDTO>(false, "Khách hàng không tồn tại");
            }
            var customerResponse = new CustomerMasterResponseDTO
            {
                CustomerCode = customer.CustomerCode,
                CustomerName = customer.CustomerName,
                CustomerAddress = customer.CustomerAddress,
                CustomerPhone = customer.CustomerPhone,
                CustomerEmail = customer.CustomerEmail
            };
            return new ServiceResponse<CustomerMasterResponseDTO>(true, "Lấy thông tin khách hàng thành công", customerResponse);
        }

        public async Task<ServiceResponse<int>> GetTotalCustomerCount()
        {
            try
            {
                var totalCount = await _customerMasterRepository.GetTotalCustomerCount();
                return new ServiceResponse<int>(true, "Lấy tổng số khách hàng thành công", totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, $"Lỗi khi lấy tổng số khách hàng: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<CustomerMasterResponseDTO>>> SearchCustomer(string textToSearch, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(textToSearch))
            {
                return new ServiceResponse<PagedResponse<CustomerMasterResponseDTO>>(false, "Từ khóa tìm kiếm không hợp lệ");
            }
            if (page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<CustomerMasterResponseDTO>>(false, "Trang hoặc kích thước trang không hợp lệ");
            }
            var customers = await _customerMasterRepository.SearchCustomer(textToSearch, page, pageSize);
            var totalCount = await _customerMasterRepository.GetTotalSearchCount(textToSearch);
            if (customers == null || customers.Count == 0)
            {
                return new ServiceResponse<PagedResponse<CustomerMasterResponseDTO>>(false, "Không tìm thấy khách hàng nào");
            }
            var lstCustomerResponse = customers.Select(c => new CustomerMasterResponseDTO
            {
                CustomerCode = c.CustomerCode,
                CustomerName = c.CustomerName,
                CustomerAddress = c.CustomerAddress,
                CustomerPhone = c.CustomerPhone,
                CustomerEmail = c.CustomerEmail
            }).ToList();
            var pagedResponse = new PagedResponse<CustomerMasterResponseDTO>(lstCustomerResponse, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<CustomerMasterResponseDTO>>(true, "Tìm kiếm khách hàng thành công", pagedResponse);
        }

        public async Task<ServiceResponse<bool>> UpdateCustomerMaster(CustomerMasterRequestDTO customer)
        {
            if (customer == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu không hợp lệ");
            }
            if (string.IsNullOrEmpty(customer.CustomerCode))
            {
                return new ServiceResponse<bool>(false, "Mã khách hàng không hợp lệ");
            }
            customer.CustomerCode = customer.CustomerCode.Trim();
            var existingCustomer = await _customerMasterRepository.GetCustomerMasterWithCustomerCode(customer.CustomerCode);
            if (existingCustomer == null)
            {
                return new ServiceResponse<bool>(false, "Khách hàng không tồn tại");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingCustomer.CustomerName = customer.CustomerName;
                    existingCustomer.CustomerPhone = customer.CustomerPhone;
                    existingCustomer.CustomerAddress = customer.CustomerAddress;
                    existingCustomer.CustomerEmail = customer.CustomerEmail;
                    await _customerMasterRepository.UpdateAsync(existingCustomer, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật khách hàng thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật khách hàng: {ex.Message}");
                }
            }
        }
    }
}
