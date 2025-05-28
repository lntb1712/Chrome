using Chrome.DTO;
using Chrome.DTO.CategoryDTO;

namespace Chrome.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<ServiceResponse<List<CategoryResponseDTO>>> GetAllCategories();
        Task<ServiceResponse<List<CategorySummaryDTO>>> GetCategorySummary();
    }
}
