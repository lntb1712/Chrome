using Chrome.DTO;
using Chrome.DTO.CategoryDTO;
using Chrome.Models;
using Chrome.Repositories.CategoryRepository;

namespace Chrome.Services.CategoryService
{
    public class CategoryService:ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;   
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<ServiceResponse<List<CategoryResponseDTO>>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetAllCategories();    
                if (categories == null || categories.Count==0)
                {
                    return new ServiceResponse<List<CategoryResponseDTO>>(false, "Không có danh mục nào");
                }
                var categoryResponse = categories.Select(c => new CategoryResponseDTO
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                }).ToList();

                return new ServiceResponse<List<CategoryResponseDTO>>(true, "Lấy danh sách danh mục thành công", categoryResponse);
            }
            catch(Exception ex)
            {
                return new ServiceResponse<List<CategoryResponseDTO>>(false, $"Lỗi khi lấy danh mục: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<CategorySummaryDTO>>> GetCategorySummary()
        {
            try
            {
                var category = await _categoryRepository.GetTotalProductInCategory();
                if (category == null)
                {
                    return new ServiceResponse<List<CategorySummaryDTO>>(false, "Danh mục không tồn tại");
                }
                var categorySummary = category.Select(p=>new  CategorySummaryDTO
                {
                    CategoryId = p.CategoryId,
                    CategoryName = p.CategoryName,
                    TotalProducts = p.ProductMasters.Count
                }).ToList();
                return new ServiceResponse<List<CategorySummaryDTO>>(true, "Lấy thông tin danh mục thành công", categorySummary);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<CategorySummaryDTO>>(false, $"Lỗi khi lấy thông tin danh mục: {ex.Message}");
            }
        }
    }
}
