using Chrome.DTO;
using Chrome.DTO.ProductMasterDTO;
using Chrome.DTO.StockOutDetailDTO;
using Chrome.Services.StockOutDetailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chrome.Controllers
{
    [Route("api/StockOut/{stockOutCode}/[controller]")]
    [ApiController]
    [Authorize(Policy = "PermissionPolicy")]
    [EnableCors("MyCors")]
    public class StockOutDetailController : ControllerBase
    {
        private readonly IStockOutDetailService _stockOutDetailService;

        public StockOutDetailController(IStockOutDetailService stockOutDetailService)
        {
            _stockOutDetailService = stockOutDetailService;
        }

        [HttpGet("GetAllStockOutDetails")]
        public async Task<IActionResult> GetAllStockOutDetails([FromRoute] string stockOutCode, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                string decodedStockOutCode = Uri.UnescapeDataString(stockOutCode);
                var response = await _stockOutDetailService.GetAllStockOutDetails(decodedStockOutCode, page, pageSize);
                if (!response.Success)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = response.Message,
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("GetListProductToSO")]
        public async Task<IActionResult> GetListProductToSO()
        {
            try
            {
                var response = await _stockOutDetailService.GetListProductToSO();
                if (!response.Success)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = response.Message,
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }
        [HttpGet("GetForecastStockOutDetail")]
        public async Task<IActionResult> GetForecastStockOutDetail([FromRoute] string stockOutCode, [FromQuery] string productCode)
        {
            try
            {
                var response = await _stockOutDetailService.GetForecastStockOutDetail(stockOutCode,productCode);
                if (!response.Success)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = response.Message,
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }

        [HttpPost("AddStockOutDetail")]
        public async Task<IActionResult> AddStockOutDetail([FromBody] StockOutDetailRequestDTO stockOutDetail)
        {
            try
            {
                var response = await _stockOutDetailService.AddStockOutDetail(stockOutDetail);
                if (!response.Success)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = response.Message,
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }

        [HttpPost("CreateBackOrder")]
        public async Task<IActionResult> CreateBackOrder([FromRoute] string stockOutCode, [FromQuery] string backOrderDescription)
        {
            try
            {
                string decodedStockOutCode = Uri.UnescapeDataString(stockOutCode);
                var response = await _stockOutDetailService.CreateBackOrder(decodedStockOutCode, backOrderDescription);
                if (!response.Success)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = response.Message,
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }

        [HttpDelete("DeleteStockOutDetail")]
        public async Task<IActionResult> DeleteStockOutDetail([FromRoute] string stockOutCode, [FromQuery] string productCode)
        {
            try
            {
                string decodedStockOutCode = Uri.UnescapeDataString(stockOutCode);
                var response = await _stockOutDetailService.DeleteStockOutDetail(decodedStockOutCode, productCode);
                if (!response.Success)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = response.Message,
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }

        [HttpPut("UpdateStockOutDetail")]
        public async Task<IActionResult> UpdateStockOutDetail([FromBody] StockOutDetailRequestDTO stockOutDetail)
        {
            try
            {
                var response = await _stockOutDetailService.UpdateStockOutDetail(stockOutDetail);
                if (!response.Success)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = response.Message,
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }

        [HttpPost("ConfirmStockOut")]
        public async Task<IActionResult> ConfirmStockOut([FromRoute] string stockOutCode)
        {
            try
            {
                string decodedStockOutCode = Uri.UnescapeDataString(stockOutCode);
                var response = await _stockOutDetailService.ConfirmStockOut(decodedStockOutCode);
                if (!response.Success)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = response.Message,
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }

        [HttpPost("CheckAndUpdateBackOrderStatus")]
        public async Task<IActionResult> CheckAndUpdateBackOrderStatus([FromRoute] string stockOutCode)
        {
            try
            {
                string decodedStockOutCode = Uri.UnescapeDataString(stockOutCode);
                var response = await _stockOutDetailService.CheckAndUpdateBackOrderStatus(decodedStockOutCode);
                if (!response.Success)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = response.Message,
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