using Chrome.DTO.ReplenishDTO;
using Chrome.Services.ReplenishService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chrome.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "PermissionPolicy")]
    [EnableCors("MyCors")]
    public class ReplenishController : ControllerBase
    {
        private readonly IReplenishService _replenishService;

        public ReplenishController(IReplenishService replenishService)
        {
            _replenishService = replenishService;
        }

        [HttpGet("GetAllReplenishAsync")]
        public async Task<IActionResult> GetAllReplenishAsync([FromQuery] string warehouseCode, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _replenishService.GetAllReplenishAsync(warehouseCode, page, pageSize);
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

        [HttpGet("GetReplenishByCodeAsync")]
        public async Task<IActionResult> GetReplenishByCodeAsync([FromQuery] string productCode, [FromQuery] string warehouseCode)
        {
            try
            {
                var response = await _replenishService.GetReplenishByCodeAsync(productCode, warehouseCode);
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

        [HttpGet("SearchReplenishAsync")]
        public async Task<IActionResult> SearchReplenishAsync([FromQuery] string warehouseCode, [FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _replenishService.SearchReplenishAsync(warehouseCode, textToSearch, page, pageSize);
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

        [HttpPost("AddReplenishAsync")]
        public async Task<IActionResult> AddReplenishAsync([FromBody] ReplenishRequestDTO replenishRequestDTO)
        {
            try
            {
                var response = await _replenishService.AddReplenishAsync(replenishRequestDTO);
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

        [HttpDelete("DeleteReplenishAsync")]
        public async Task<IActionResult> DeleteReplenishAsync([FromQuery] string productCode, [FromQuery] string warehouseCode)
        {
            try
            {
                var response = await _replenishService.DeleteReplenishAsync(productCode, warehouseCode);
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

        [HttpPut("UpdateReplenishAsync")]
        public async Task<IActionResult> UpdateReplenishAsync([FromBody] ReplenishRequestDTO replenishRequestDTO)
        {
            try
            {
                var response = await _replenishService.UpdateReplenishAsync(replenishRequestDTO);
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
        [HttpGet("GetTotalReplenishCountAsync")]
        public async Task<IActionResult> GetTotalReplenishCountAsync([FromQuery] string warehouseCode)
        {
            try
            {
                var response = await _replenishService.GetTotalReplenishCountAsync(warehouseCode);
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

        [HttpGet("CheckReplenishWarningsAsync")]
        public async Task<IActionResult> CheckReplenishWarningsAsync([FromQuery] string warehouseCode)
        {
            try
            {
                var response = await _replenishService.CheckReplenishWarningsAsync(warehouseCode);
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

        [HttpGet("GetListProductToReplenish")]
        public async Task<IActionResult> GetListProductToReplenish()
        {
            try
            {
                var response = await _replenishService.GetListProductToReplenish();
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
