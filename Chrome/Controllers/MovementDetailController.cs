using Chrome.DTO.MovementDetailDTO;
using Chrome.Services.MovementDetailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Chrome.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "PermissionPolicy")]
    [EnableCors("MyCors")]
    public class MovementDetailController : ControllerBase
    {
        private readonly IMovementDetailService _movementDetailService;

        public MovementDetailController(IMovementDetailService movementDetailService)
        {
            _movementDetailService = movementDetailService ?? throw new ArgumentNullException(nameof(movementDetailService));
        }

        [HttpGet("GetAllMovementDetails")]
        public async Task<IActionResult> GetAllMovementDetails([FromQuery] string[] warehouseCodes, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _movementDetailService.GetAllMovementDetailsAsync(warehouseCodes, page, pageSize);
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

        [HttpGet("GetMovementDetailsByMovementCode")]
        public async Task<IActionResult> GetMovementDetailsByMovementCode([FromQuery] string movementCode, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                string decodedMovementCode = Uri.UnescapeDataString(movementCode);
                var response = await _movementDetailService.GetMovementDetailsByMovementCodeAsync(decodedMovementCode, page, pageSize);
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

        [HttpGet("SearchMovementDetails")]
        public async Task<IActionResult> SearchMovementDetails([FromQuery] string[] warehouseCodes, [FromQuery] string movementCode, [FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                string decodedMovementCode = Uri.UnescapeDataString(movementCode);
                var response = await _movementDetailService.SearchMovementDetailsAsync(warehouseCodes, decodedMovementCode, textToSearch, page, pageSize);
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

        [HttpPost("AddMovementDetail")]
        public async Task<IActionResult> AddMovementDetail([FromBody] MovementDetailRequestDTO movementDetail)
        {
            try
            {
                var response = await _movementDetailService.AddMovementDetail(movementDetail);
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

        [HttpPut("UpdateMovementDetail")]
        public async Task<IActionResult> UpdateMovementDetail([FromBody] MovementDetailRequestDTO movementDetail)
        {
            try
            {
                var response = await _movementDetailService.UpdateMovementDetail(movementDetail);
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

        [HttpDelete("DeleteMovementDetail")]
        public async Task<IActionResult> DeleteMovementDetail([FromQuery] string movementCode, [FromQuery] string productCode)
        {
            try
            {
                string decodedMovementCode = Uri.UnescapeDataString(movementCode);
                var response = await _movementDetailService.DeleteMovementDetail(decodedMovementCode, productCode);
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

        [HttpGet("GetProductByLocationCode")]
        public async Task<IActionResult> GetProductByLocationCode([FromQuery] string locationCode)
        {
            try
            {
                var response = await _movementDetailService.GetProductByLocationCode(locationCode);
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