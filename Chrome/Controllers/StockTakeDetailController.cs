using Chrome.DTO.StockTakeDetailDTO;
using Chrome.Services.StockTakeDetailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chrome.Controllers
{
    [Route("api/StockTake/{stockTakeCode}/[controller]")]
    [ApiController]
    [Authorize(Policy = "PermissionPolicy")]
    [EnableCors("MyCors")]
    public class StockTakeDetailController : ControllerBase
    {
        private readonly IStockTakeDetailService _StockTakeDetailService;

        public StockTakeDetailController(IStockTakeDetailService StockTakeDetailService)
        {
            _StockTakeDetailService = StockTakeDetailService ?? throw new ArgumentNullException(nameof(StockTakeDetailService));
        }

        [HttpGet("GetAllStockTakeDetails")]
        public async Task<IActionResult> GetAllStockTakeDetails([FromQuery] string[] warehouseCodes, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _StockTakeDetailService.GetAllStockTakeDetailsAsync(warehouseCodes, page, pageSize);
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

        [HttpGet("GetStockTakeDetailsByStockTakeCode")]
        public async Task<IActionResult> GetStockTakeDetailsByStockTakeCode([FromRoute] string StockTakeCode, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _StockTakeDetailService.GetStockTakeDetailsByStockTakeCodeAsync(StockTakeCode, page, pageSize);
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

        [HttpGet("SearchStockTakeDetails")]
        public async Task<IActionResult> SearchStockTakeDetails([FromQuery] string[] warehouseCodes, [FromRoute] string StockTakeCode, [FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _StockTakeDetailService.SearchStockTakeDetailsAsync(warehouseCodes, StockTakeCode, textToSearch, page, pageSize);
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

        [HttpPut("UpdateStockTakeDetail")]
        public async Task<IActionResult> UpdateStockTakeDetail([FromBody] StockTakeDetailRequestDTO StockTakeDetail)
        {
            try
            {
                var response = await _StockTakeDetailService.UpdateStockTakeDetail(StockTakeDetail);
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

        [HttpDelete("DeleteStockTakeDetail")]
        public async Task<IActionResult> DeleteStockTakeDetail([FromRoute] string StockTakeCode, [FromQuery] string productCode, [FromQuery] string lotNo, [FromQuery] string locationCode)
        {
            try
            {
                var response = await _StockTakeDetailService.DeleteStockTakeDetail(StockTakeCode, productCode, lotNo, locationCode);
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
    }
}
