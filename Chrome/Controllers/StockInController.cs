using Chrome.DTO.StockInDTO;
using Chrome.Services.StockInService;
using DocumentFormat.OpenXml.Wordprocessing;
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
    public class StockInController : ControllerBase
    {
        private readonly IStockInService _stockInService;
        public StockInController(IStockInService stockInService)
        {
            _stockInService = stockInService;
        }

        [HttpGet("GetAllStockIns")]
        public async Task<IActionResult> GetAllStockIns([FromQuery] string[] warehouseCodes, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _stockInService.GetAllStockIns(warehouseCodes, page, pageSize);
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

        [HttpGet("GetAllStockInWithResponsible")]
        public async Task<IActionResult> GetAllStockInWithResponsible([FromQuery] string[] warehouseCodes,[FromQuery]string responsible, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _stockInService.GetAllStockInWithResponsible(warehouseCodes,responsible, page, pageSize);
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

        [HttpGet("GetAllStockInsWithStatus")]
        public async Task<IActionResult> GetAllStockInsWithStatus([FromQuery] string[] warehouseCodes, [FromQuery] int statusId, [FromQuery] int page=1, [FromQuery] int pageSize=10)
        {
            try
            {
                var response = await _stockInService.GetAllStockInsWithStatus(warehouseCodes, statusId, page, pageSize);
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

        [HttpGet("SearchStockInAsync")]
        public async Task<IActionResult> SearchStockInAsync([FromQuery] string[] warehouseCodes, [FromQuery] string textToSearch, [FromQuery] int page=1, [FromQuery] int pageSize=10)
        {
            try
            {
                var response = await _stockInService.SearchStockInAsync(warehouseCodes, textToSearch, page, pageSize);
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
        [HttpGet("SearchStockInWithResponsible")]
        public async Task<IActionResult> SearchStockInWithResponsible([FromQuery] string[] warehouseCodes,[FromQuery]string responsible, [FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _stockInService.SearchStockInWithResponsible(warehouseCodes,responsible, textToSearch, page, pageSize);
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
                var response = await _stockInService.GetListOrderType(prefix);
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

        [HttpGet("GetListPurchaseOrder")]
        public async Task<IActionResult> GetListPurchaseOrder([FromQuery]string[] warehouseCodes, [FromQuery] int[] statusFilters)
        {
            try
            {
                var response = await _stockInService.GetListPurchaseOrder(warehouseCodes,statusFilters);
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
                var response = await _stockInService.GetListResponsibleAsync(warehouseCode);
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
                var response = await _stockInService.GetListStatusMaster();
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
        public async Task<IActionResult> GetListWarehousePermission([FromQuery] string [] warehouseCodes)
        {
            try
            {
                var response = await _stockInService.GetListWarehousePermission(warehouseCodes);
                if(!response.Success)
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
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi : {ex.Message}");
            }
        }

        [HttpGet("GetStockInByCode")]
        public async Task<IActionResult> GetStockInByCode([FromQuery]string stockInCode)
        {
            try
            {
                string decodedStockInCode = Uri.UnescapeDataString(stockInCode);
                var response = await _stockInService.GetStockInByCode(decodedStockInCode);
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

        [HttpPost("AddStockIn")]
        public async Task<IActionResult> AddStockIn([FromBody] StockInRequestDTO stockIn)
        {
            try
            {
                var response = await _stockInService.AddStockIn(stockIn);
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
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi : {ex.Message}");
            }
        }

        [HttpDelete("DeleteStockInAsync")]
        public async Task<IActionResult> DeleteStockInAsync([FromQuery] string stockInCode)
        {
            try
            {
                string decodedStockInCode = Uri.UnescapeDataString(stockInCode);
                var response = await _stockInService.DeleteStockInAsync(decodedStockInCode);
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

        [HttpPut("UpdateStockIn")]        
        public async Task<IActionResult> UpdateStockIn([FromBody] StockInRequestDTO stockIn)
        {
            try
            {
                var response = await _stockInService.UpdateStockIn(stockIn);
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
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi : {ex.Message}");
            }
        }
    }
}
