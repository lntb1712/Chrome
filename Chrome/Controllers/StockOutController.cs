using Chrome.DTO;
using Chrome.DTO.StockOutDTO;
using Chrome.Services.StockOutService;
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
    public class StockOutController : ControllerBase
    {
        private readonly IStockOutService _stockOutService;

        public StockOutController(IStockOutService stockOutService)
        {
            _stockOutService = stockOutService;
        }

        [HttpGet("GetAllStockOuts")]
        public async Task<IActionResult> GetAllStockOuts([FromQuery] string[] warehouseCodes, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _stockOutService.GetAllStockOuts(warehouseCodes, page, pageSize);
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
        [HttpGet("GetAllStockOutsWithResponsible")]
        public async Task<IActionResult> GetAllStockOutsWithResponsible([FromQuery] string[] warehouseCodes,[FromQuery]string responsible, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _stockOutService.GetAllStockOutsWithResponsible(warehouseCodes,responsible, page, pageSize);
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

        [HttpGet("GetAllStockOutsWithStatus")]
        public async Task<IActionResult> GetAllStockOutsWithStatus([FromQuery] string[] warehouseCodes, [FromQuery] int statusId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _stockOutService.GetAllStockOutsWithStatus(warehouseCodes, statusId, page, pageSize);
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

        [HttpGet("SearchStockOutAsync")]
        public async Task<IActionResult> SearchStockOutAsync([FromQuery] string[] warehouseCodes, [FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _stockOutService.SearchStockOutAsync(warehouseCodes, textToSearch, page, pageSize);
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
        [HttpGet("SearchStockOutAsyncWithResponsible")]
        public async Task<IActionResult> SearchStockOutAsyncWithResponsible([FromQuery] string[] warehouseCodes,[FromQuery]string responsible, [FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _stockOutService.SearchStockOutAsyncWithResponsible(warehouseCodes,responsible, textToSearch, page, pageSize);
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

        [HttpGet("GetListOrderType")]
        public async Task<IActionResult> GetListOrderType([FromQuery] string prefix)
        {
            try
            {
                var response = await _stockOutService.GetListOrderType(prefix);
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

        [HttpGet("GetListCustomerMasterAsync")]
        public async Task<IActionResult> GetListCustomerMasterAsync()
        {
            try
            {
                var response = await _stockOutService.GetListCustomerMasterAsync();
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

        [HttpGet("GetListResponsibleAsync")]
        public async Task<IActionResult> GetListResponsibleAsync([FromQuery]string warehouseCode)
        {
            try
            {
                var response = await _stockOutService.GetListResponsibleAsync(warehouseCode);
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

        [HttpGet("GetListStatusMaster")]
        public async Task<IActionResult> GetListStatusMaster()
        {
            try
            {
                var response = await _stockOutService.GetListStatusMaster();
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

        [HttpGet("GetListWarehousePermission")]
        public async Task<IActionResult> GetListWarehousePermission([FromQuery] string[] warehouseCodes)
        {
            try
            {
                var response = await _stockOutService.GetListWarehousePermission(warehouseCodes);
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

        [HttpPost("AddStockOut")]
        public async Task<IActionResult> AddStockOut([FromBody] StockOutRequestDTO stock)
        {
            try
            {
                var response = await _stockOutService.AddStockOut(stock);
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

        [HttpDelete("DeleteStockOutAsync")]
        public async Task<IActionResult> DeleteStockOutAsync([FromQuery] string stockOutCode)
        {
            try
            {
                string decodedStockOutCode = Uri.UnescapeDataString(stockOutCode);
                var response = await _stockOutService.DeleteStockOutAsync(decodedStockOutCode);
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

        [HttpPut("UpdateStockOut")]
        public async Task<IActionResult> UpdateStockOut([FromBody] StockOutRequestDTO stock)
        {
            try
            {
                var response = await _stockOutService.UpdateStockOut(stock);
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