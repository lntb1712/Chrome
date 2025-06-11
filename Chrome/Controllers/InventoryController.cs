using Chrome.DTO;
using Chrome.DTO.InventoryDTO;
using Chrome.Services.CategoryService;
using Chrome.Services.InventoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chrome.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy ="PermissionPolicy")]
    [EnableCors("MyCors")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ICategoryService _categoryService;

        public InventoryController(IInventoryService inventoryService, ICategoryService categoryService)
        {
            _inventoryService = inventoryService;
            _categoryService = categoryService;
        }

        [HttpGet("GetListProductInventory")]
        public async Task<IActionResult> GetListProductInventory([FromQuery] string[] warehouseCodes, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _inventoryService.GetListProductInventory(warehouseCodes, page, pageSize);
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

        [HttpGet("GetProductWithLocations")]
        public async Task<IActionResult> GetProductWithLocations([FromQuery] string[] warehouseCodes,[FromQuery] string productCode, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _inventoryService.GetProductWithLocationsAsync(warehouseCodes,productCode, page, pageSize);
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

        [HttpGet("GetListProductInventoryByCategoryIds")]
        public async Task<IActionResult> GetListProductInventoryByCategoryIds([FromQuery] string[] warehouseCodes, [FromQuery] string[] categoryIds, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _inventoryService.GetListProductInventoryByCategoryIds(warehouseCodes, categoryIds, page, pageSize);
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

        [HttpGet("SearchProductInventory")]
        public async Task<IActionResult> SearchProductInventory([FromQuery] string[] warehouseCodes, [FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _inventoryService.SearchProductInventoryAsync(warehouseCodes, textToSearch, page, pageSize);
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

        [HttpPost("AddInventory")]
        public async Task<IActionResult> AddInventory([FromBody] InventoryRequestDTO inventoryRequestDTO)
        {
            try
            {
                var response = await _inventoryService.AddInventory(inventoryRequestDTO);
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

        [HttpPut("UpdateInventory")]
        public async Task<IActionResult> UpdateInventory([FromBody] InventoryRequestDTO inventoryRequestDTO)
        {
            try
            {
                var response = await _inventoryService.UpdateInventoryAsync(inventoryRequestDTO);
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

        [HttpDelete("DeleteInventory")]
        public async Task<IActionResult> DeleteInventory([FromQuery] string warehouseCode, [FromQuery] string locationCode, [FromQuery] string productCode, [FromQuery] string lotNo)
        {
            try
            {
                var response = await _inventoryService.DeleteInventoryAsync(warehouseCode, locationCode, productCode, lotNo);
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
