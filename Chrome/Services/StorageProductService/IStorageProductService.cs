using Chrome.DTO;
using Chrome.DTO.StorageProductDTO;

namespace Chrome.Services.StorageProductService
{
    public interface IStorageProductService 
    {
        Task<ServiceResponse<PagedResponse<StorageProductResponseDTO>>> GetAllStorageProducts(int page, int pageSize);
        Task<ServiceResponse<StorageProductResponseDTO>> GetStorageProductWithCode(string storageProductCode);
        Task<ServiceResponse<int>> GetTotalStorageProductCount();
        Task<ServiceResponse<PagedResponse<StorageProductResponseDTO>>> SearchStorageProducts(string textToSearch, int page, int pageSize);
        Task<ServiceResponse<bool>> AddStorageProduct(StorageProductRequestDTO storageProductRequestDTO);
        Task<ServiceResponse<bool>> DeleteStorageProduct(string storageProductCode);
        Task<ServiceResponse<bool>> UpdateStorageProduct(StorageProductRequestDTO storageProductRequestDTO);
    }
}
