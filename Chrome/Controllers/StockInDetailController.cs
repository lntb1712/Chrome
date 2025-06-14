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
    [Route("api/StockIn/{stockInCode}/StockInDetail/[controller]")]
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
        public async Task<IActionResult> GetAllStockInDetails([FromQuery] string stockInCode, [FromQuery] int page=1, [FromQuery] int pageSize=10)
        {
            try
            {
                var response = await _stockInDetailService.GetAllStockInDetails(stockInCode, page, pageSize);
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
        public async Task<IActionResult> CreateBackOrder([FromQuery]string stockInCode, [FromQuery]string backOrderDescription)
        {
            try
            {
                var response = await _stockInDetailService.CreateBackOrder(stockInCode,backOrderDescription);
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
        public async Task<IActionResult> DeleteStockInDetail([FromQuery]string stockInCode, [FromQuery]string productCode)
        {
            try
            {
                var response = await _stockInDetailService.DeleteStockInDetail(stockInCode, productCode);
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

        [HttpPut("ConfirmStockIn")]
        public async Task<IActionResult> ConfirmStockIn([FromQuery] string stockInCode)
        {
            try
            {
                var response = await _stockInDetailService.ConfirmStockIn(stockInCode);
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

        [HttpPut("CheckAndUpdateBackOrderStatus")]
        public async Task<IActionResult> CheckAndUpdateBackOrderStatus([FromQuery]string stockInCode)
        {
            try
            {
                var response = await _stockInDetailService.CheckAndUpdateBackOrderStatus(stockInCode);
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
