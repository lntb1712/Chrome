using Chrome.DTO;
using Chrome.DTO.SupplierMasterDTO;
using Chrome.Models;
using Chrome.Repositories.SupplierMasterRepository;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Services.SupplierMasterService
{
    public class SupplierMasterService : ISupplierMasterService
    {
        private readonly ISupplierMasterRepository _supplierMasterRepository;
        private readonly ChromeContext _context;
        
        public SupplierMasterService(ISupplierMasterRepository supplierMasterRepository, ChromeContext context)
        {
            _supplierMasterRepository = supplierMasterRepository;
            _context= context;
        }

        public async Task<ServiceResponse<bool>> AddSupplierMaster(SupplierMasterRequestDTO supplier)
        {
            if(supplier ==null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var supplierMaster = new SupplierMaster
            {
                SupplierCode = supplier.SupplierCode,
                SupplierName = supplier.SupplierName,
                SupplierPhone = supplier.SupplierPhone,
                SupplierAddress = supplier.SupplierAddress,
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _supplierMasterRepository.AddAsync(supplierMaster, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm nhà cung cấp thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi thêm sản phẩm: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteSupplierMaster(string supplierCode)
        {
            if(string.IsNullOrEmpty(supplierCode))
            {
                return new ServiceResponse<bool>(false, "Mã nhà cung cấp không được để trống");
            }    

            var supplier = await _supplierMasterRepository.GetSupplierMasterWithSupplierCode(supplierCode);
            if (supplier == null)
            {
                return new ServiceResponse<bool>(false, "Nhà cung cấp không tồn tại");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _supplierMasterRepository.DeleteAsync(supplierCode, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa nhà cung cấp thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa nhà cung cấp vì có dữ liệu tham chiếu.");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa nhà cung cấp: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<SupplierMasterResponseDTO>>> GetAllSupplierMaster(int page, int pageSize)
        {
            if(page<1 ||pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<SupplierMasterResponseDTO>>(false, "Trang và kích thước không hợp lệ");
            }  
            
            var lstSupplier = await _supplierMasterRepository.GetAllSupplier(page, pageSize);
            var totalSupplierCount = await _supplierMasterRepository.GetTotalSupplierCount();

            if(lstSupplier==null || lstSupplier.Count == 0)
            {
                return new ServiceResponse<PagedResponse<SupplierMasterResponseDTO>>(false, "Không có nhà cung cấp nào");
            }    

            var supplierResponse = lstSupplier.Select(x=>new SupplierMasterResponseDTO
            {
                SupplierCode= x.SupplierCode,
                SupplierName= x.SupplierName,
                SupplierAddress= x.SupplierAddress,
                SupplierPhone= x.SupplierPhone,
            }).ToList();

            var pagedResponse = new PagedResponse<SupplierMasterResponseDTO>(supplierResponse,page,pageSize,totalSupplierCount);
            return new ServiceResponse<PagedResponse<SupplierMasterResponseDTO>>(true, "Lấy danh sách nhà cung cấp thành công", pagedResponse);

        }

        public async Task<ServiceResponse<SupplierMasterResponseDTO>> GetSupplierWithSupplierCode(string supplierCode)
        {
            if(string.IsNullOrEmpty(supplierCode))
            {
                return new ServiceResponse<SupplierMasterResponseDTO>(false, "Mã nhà cung cấp không được để trống");
            }

            var supplier = await _supplierMasterRepository.GetSupplierMasterWithSupplierCode(supplierCode);
            if(supplier == null)
            {
                return new ServiceResponse<SupplierMasterResponseDTO>(false, "Nhà cung cấp không tồn tại");
            }
            var supplierResponse = new SupplierMasterResponseDTO
            {
                SupplierCode = supplierCode,
                SupplierName = supplier.SupplierName,
                SupplierAddress = supplier.SupplierAddress,
                SupplierPhone = supplier.SupplierPhone,
            };

            return new ServiceResponse<SupplierMasterResponseDTO>(true, "Lấy thông tin nhà cung cấp thành công", supplierResponse);
        }

        public async Task<ServiceResponse<int>> GetTotalSupplierCount()
        {
            try
            {
                var totalAccount = await _supplierMasterRepository.GetTotalSupplierCount();
                return new ServiceResponse<int>(true, "Lấy tổng danh sách nhà cung cấp", totalAccount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false,"Lỗi" +ex.Message);
            }
        }

        public async Task<ServiceResponse<PagedResponse<SupplierMasterResponseDTO>>> SearchSupplier(string textToSearch, int page, int pageSize)
        {
            if(string.IsNullOrEmpty(textToSearch)|| page<1 || pageSize <1)
            {
                return new ServiceResponse<PagedResponse<SupplierMasterResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }    
            var lstSupplier = await _supplierMasterRepository.SearchSupplier(textToSearch, page, pageSize);
            var totalSearchCount = await _supplierMasterRepository.GetTotalSearchCount(textToSearch);

            if(lstSupplier==null || totalSearchCount == 0)
            {
                return new ServiceResponse<PagedResponse<SupplierMasterResponseDTO>>(false, "Không có nhà cung cấp nào");
            }

            var supplierResponse = lstSupplier.Select(x => new SupplierMasterResponseDTO
            {
                SupplierCode = x.SupplierCode,
                SupplierName = x.SupplierName,
                SupplierAddress = x.SupplierAddress,
                SupplierPhone = x.SupplierPhone,
            }).ToList();

            var pagedResponse = new PagedResponse<SupplierMasterResponseDTO>(supplierResponse,page,pageSize,totalSearchCount);
            return new ServiceResponse<PagedResponse<SupplierMasterResponseDTO>>(true, "Lấy danh sách nhà cung cấp đã lọc", pagedResponse);
                
        }

        public async Task<ServiceResponse<bool>> UpdateSupplierMaster(SupplierMasterRequestDTO supplier)
        {
            if(supplier == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var existingSupplier = await _supplierMasterRepository.GetSupplierMasterWithSupplierCode(supplier.SupplierCode);
            if (existingSupplier == null)
            {
                return new ServiceResponse<bool>(false, "Nhà cung cấp không tồn tại");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingSupplier.SupplierName = supplier.SupplierName;
                    existingSupplier.SupplierPhone = supplier.SupplierPhone;
                    existingSupplier.SupplierAddress = supplier.SupplierAddress;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật thông tin nhà cung cấp thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật nhà cung cấp: {ex.Message}");
                }
            }
        }
    }
}
