using Chrome.DTO.StockInDetailDTO;
using Chrome.Models;
using Chrome.Services.StockInDetailService;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chrome.Controllers
{
    [Route("api/StockIn/{stockInCode}/[controller]")]
    [ApiController]
    [Authorize(Policy ="PermissionPolicy")]
    [EnableCors("MyCors")]
    public class StockInDetailController : ControllerBase
    {
        private readonly IStockInDetailService _stockInDetailService;
        public StockInDetailController(IStockInDetailService stockInDetailService)
        {
            _stockInDetailService = stockInDetailService;
        }

        [HttpGet("GetAllStockInDetails")]
        public async Task<IActionResult> GetAllStockInDetails([FromRoute] string stockInCode, [FromQuery] int page=1, [FromQuery] int pageSize=10)
        {
            try
            {
                // Giải mã stockInCode
                string decodedStockInCode = Uri.UnescapeDataString(stockInCode);
                var response = await _stockInDetailService.GetAllStockInDetails(decodedStockInCode, page, pageSize);
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

        [HttpGet("GetListProductToSI")]
        public async Task<IActionResult> GetListProductToSI()
        {
            try
            {
                var response = await _stockInDetailService.GetListProductToSI();
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

        [HttpPost("AddStockInDetail")]
        public async Task<IActionResult> AddStockInDetail([FromBody] StockInDetailRequestDTO stockInDetail)
        {
            try
            {
                var response = await _stockInDetailService.AddStockInDetail(stockInDetail);
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
        public async Task<IActionResult> CreateBackOrder([FromRoute]string stockInCode, [FromQuery]string backOrderDescription)
        {
            try
            {
                string decodedStockInCode = Uri.UnescapeDataString(stockInCode);
                var response = await _stockInDetailService.CreateBackOrder(decodedStockInCode, backOrderDescription);
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

        [HttpDelete("DeleteStockInDetail")]        
        public async Task<IActionResult> DeleteStockInDetail([FromRoute]string stockInCode, [FromQuery]string productCode)
        {
            try
            {
                // Giải mã stockInCode
                string decodedStockInCode = Uri.UnescapeDataString(stockInCode);
                var response = await _stockInDetailService.DeleteStockInDetail(decodedStockInCode, productCode);
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

        [HttpPut("UpdateStockInDetail")]
        public async Task<IActionResult> UpdateStockInDetail([FromBody]StockInDetailRequestDTO stockInDetail)
        {
            try
            {
                var response = await _stockInDetailService.UpdateStockInDetail(stockInDetail);
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

        [HttpPost("ConfirmStockIn")]
        public async Task<IActionResult> ConfirmStockIn([FromRoute] string stockInCode)
        {
            try
            {
                string decodedStockInCode = Uri.UnescapeDataString(stockInCode);
                var response = await _stockInDetailService.ConfirmStockIn(decodedStockInCode);
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
        [HttpPost("CreatePutAway")]
        public async Task<IActionResult> CreatePutAway([FromRoute] string stockInCode)
        {
            try
            {
                string decodedStockInCode = Uri.UnescapeDataString(stockInCode);
                var response = await _stockInDetailService.CreatePutAway(decodedStockInCode);
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
        public async Task<IActionResult> CheckAndUpdateBackOrderStatus([FromRoute]string stockInCode)
        {
            try
            {
                string decodedStockInCode = Uri.UnescapeDataString(stockInCode);
                var response = await _stockInDetailService.CheckAndUpdateBackOrderStatus(decodedStockInCode);
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
