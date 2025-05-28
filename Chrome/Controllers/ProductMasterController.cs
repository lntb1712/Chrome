using Chrome.DTO.ProductMasterDTO;
using Chrome.Services.CategoryService;
using Chrome.Services.ProductMasterService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chrome.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductMasterController : ControllerBase
    {
        private readonly IProductMasterService _productMasterService;
        private readonly ICategoryService _categoryService;
        public ProductMasterController(IProductMasterService productMasterService, ICategoryService categoryService)
        {
            _productMasterService = productMasterService;
            _categoryService = categoryService;
        }
        // ProductMaster API
        [HttpGet("GetAllProductMaster")]
        public async Task<IActionResult> GetAllProductMaster([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _productMasterService.GetAllProductMaster(page, pageSize);
                if (!response.Success)
                {
                    return NotFound(new
                    {
                        Succcess = false,
                        Message = response.Message
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("GetProductMasterWithProductCode")]
        public async Task<IActionResult> GetProductMasterWithProductCode([FromQuery] string productCode)
        {
            try
            {
                var response = await _productMasterService.GetProductMasterWithProductCode(productCode);
                if (!response.Success)
                {
                    return NotFound(new
                    {
                        Succcess = false,
                        Message = response.Message
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("GetAllProductWithCategoryID")]
        public async Task<IActionResult> GetAllProductWithCategoryID([FromQuery] string categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _productMasterService.GetAllProductWithCategoryID(categoryId, page, pageSize);
                if (!response.Success)
                {
                    return NotFound(new
                    {
                        Succcess = false,
                        Message = response.Message
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("SearchProduct")]
        public async Task<IActionResult> SearchProduct([FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _productMasterService.SearchProduct(textToSearch, page, pageSize);
                if (!response.Success)
                {
                    return NotFound(new
                    {
                        Succcess = false,
                        Message = response.Message
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }

        [HttpPost("AddProductMaster")]
        public async Task<IActionResult> AddProductMaster([FromBody] ProductMasterRequestDTO product)
        {
            try
            {
                var response = await _productMasterService.AddProductMaster(product);
                if (!response.Success)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = response.Message
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }

        [HttpPut("UpdateProductMaster")]
        public async Task<IActionResult> UpdateProductMaster([FromBody] ProductMasterRequestDTO product)
        {
            try
            {
                var response = await _productMasterService.UpdateProductMaster(product);
                if (!response.Success)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = response.Message
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }

        [HttpDelete("DeleteProductMaster")]
        public async Task<IActionResult> DeleteProductMaster([FromQuery] string productCode)
        {
            try
            {
                var response = await _productMasterService.DeleteProductMaster(productCode);
                if (!response.Success)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = response.Message
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("GetTotalProductCount")]
        public async Task<IActionResult> GetTotalProductCount()
        {
            try
            {
                var response = await _productMasterService.GetTotalProductCount();
                if (!response.Success)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = response.Message
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }

        // Category API
        [HttpGet("GetCategorySummary")]
        public async Task<IActionResult> GetCategorySummary()
        {
            try
            {
                var response = await _categoryService.GetCategorySummary();
                if (!response.Success)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = response.Message
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var response = await _categoryService.GetAllCategories();
                if (!response.Success)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = response.Message
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }
    }
}
