using Chrome.DTO.WarehouseMasterDTO;
using Chrome.Services.WarehouseMasterService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chrome.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy="PermissionPolicy")]
    [EnableCors("MyCors")]  

    public class WarehouseMasterController : ControllerBase
    {
        private readonly IWarehouseMasterService _warehouseMasterService;
        public WarehouseMasterController(IWarehouseMasterService warehouseMasterService)
        {
            _warehouseMasterService = warehouseMasterService;
        }
        // WarehouseMaster API
        [HttpGet("GetAllWarehouseMaster")]
        public async Task<IActionResult> GetAllWarehouseMaster([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _warehouseMasterService.GetAllWarehouseMaster(page, pageSize);
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
        [HttpGet("GetWarehouseMasterWithCode")]
        public async Task<IActionResult> GetWarehouseMasterWithCode([FromQuery] string warehouseCode)
        {
            try
            {
                var response = await _warehouseMasterService.GetWarehouseMasterWithCode(warehouseCode);
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
        [HttpGet("SearchWarehouse")]
        public async Task<IActionResult> SearchWarehouse([FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _warehouseMasterService.SearchWarehouse(textToSearch, page, pageSize);
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
        [HttpPost("AddWarehouseMaster")]
        public async Task<IActionResult> AddWarehouseMaster([FromBody] WarehouseMasterRequestDTO warehouse)
        {
            try
            {
                var response = await _warehouseMasterService.AddWarehouseMaster(warehouse);
                if (!response.Success)
                {
                    return BadRequest(new
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
        [HttpPut("UpdateWarehouseMaster")]
        public async Task<IActionResult> UpdateWarehouseMaster([FromBody] WarehouseMasterRequestDTO warehouse)
        {
            try
            {
                var response = await _warehouseMasterService.UpdateWarehouseMaster(warehouse);
                if (!response.Success)
                {
                    return BadRequest(new
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
        [HttpDelete("DeleteWarehouseMaster")]
        public async Task<IActionResult> DeleteWarehouseMaster([FromQuery] string warehouseCode)
        {
            try
            {
                var response = await _warehouseMasterService.DeleteWarehouseMaster(warehouseCode);
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
        [HttpGet("GetTotalWarehouseCount")]
        public async Task<IActionResult> GetTotalWarehouseCount()
        {
            try
            {
                var response = await _warehouseMasterService.GetTotalWarehouseCount();
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
