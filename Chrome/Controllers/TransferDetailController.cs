using Chrome.DTO;
using Chrome.DTO.InventoryDTO;
using Chrome.DTO.TransferDetailDTO;
using Chrome.Services.TransferDetailService;
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
    public class TransferDetailController : ControllerBase
    {
        private readonly ITransferDetailService _transferDetailService;

        public TransferDetailController(ITransferDetailService transferDetailService)
        {
            _transferDetailService = transferDetailService ?? throw new ArgumentNullException(nameof(transferDetailService));
        }

        [HttpGet("GetAllTransferDetails")]
        public async Task<IActionResult> GetAllTransferDetails([FromQuery] string[] warehouseCodes, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _transferDetailService.GetAllTransferDetailsAsync(warehouseCodes, page, pageSize);
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

        [HttpGet("GetTransferDetailsByTransferCode")]
        public async Task<IActionResult> GetTransferDetailsByTransferCode([FromQuery] string transferCode, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _transferDetailService.GetTransferDetailsByTransferCodeAsync(transferCode, page, pageSize);
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

        [HttpGet("SearchTransferDetails")]
        public async Task<IActionResult> SearchTransferDetails([FromQuery] string[] warehouseCodes, [FromQuery] string transferCode, [FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _transferDetailService.SearchTransferDetailsAsync(warehouseCodes, transferCode, textToSearch, page, pageSize);
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

        [HttpPost("AddTransferDetail")]
        public async Task<IActionResult> AddTransferDetail([FromBody] TransferDetailRequestDTO transferDetail)
        {
            try
            {
                var response = await _transferDetailService.AddTransferDetail(transferDetail);
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

        [HttpPut("UpdateTransferDetail")]
        public async Task<IActionResult> UpdateTransferDetail([FromBody] TransferDetailRequestDTO transferDetail)
        {
            try
            {
                var response = await _transferDetailService.UpdateTransferDetail(transferDetail);
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

        [HttpDelete("DeleteTransferDetail")]
        public async Task<IActionResult> DeleteTransferDetail([FromQuery] string transferCode, [FromQuery] string productCode)
        {
            try
            {
                var response = await _transferDetailService.DeleteTransferDetail(transferCode, productCode);
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

        [HttpGet("GetProductByWarehouseCode")]
        public async Task<IActionResult> GetProductByWarehouseCode([FromQuery] string warehouseCode)
        {
            try
            {
                var response = await _transferDetailService.GetProductByWarehouseCode(warehouseCode);
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