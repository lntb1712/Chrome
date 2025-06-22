using Chrome.DTO;
using Chrome.DTO.PutAwayDetailDTO;
using Chrome.Services.PutAwayDetailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chrome.Controllers
{
    [Route("api/PutAway/{putAwayCode}/[controller]")]
    [ApiController]
    [Authorize(Policy = "PermissionPolicy")]
    [EnableCors("MyCors")]
    public class PutAwayDetailController : ControllerBase
    {
        private readonly IPutAwayDetailService _putAwayDetailService;

        public PutAwayDetailController(IPutAwayDetailService putAwayDetailService)
        {
            _putAwayDetailService = putAwayDetailService ?? throw new ArgumentNullException(nameof(putAwayDetailService));
        }

        [HttpGet("GetAllPutAwayDetails")]
        public async Task<IActionResult> GetAllPutAwayDetails([FromQuery] string[] warehouseCodes, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _putAwayDetailService.GetAllPutAwayDetailsAsync(warehouseCodes, page, pageSize);
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

        [HttpGet("GetPutAwayDetailsByPutawayCode")]
        public async Task<IActionResult> GetPutAwayDetailsByPutawayCode([FromRoute] string putAwayCode, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _putAwayDetailService.GetPutAwayDetailsByPutawayCodeAsync(putAwayCode, page, pageSize);
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

        [HttpGet("SearchPutAwayDetails")]
        public async Task<IActionResult> SearchPutAwayDetails([FromQuery] string[] warehouseCodes, [FromRoute] string putAwayCode, [FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _putAwayDetailService.SearchPutAwayDetailsAsync(warehouseCodes, putAwayCode, textToSearch, page, pageSize);
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

        [HttpPut("UpdatePutAwayDetail")]
        public async Task<IActionResult> UpdatePutAwayDetail([FromBody] PutAwayDetailRequestDTO putAwayDetail)
        {
            try
            {
                var response = await _putAwayDetailService.UpdatePutAwayDetail(putAwayDetail);
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

        [HttpDelete("DeletePutAwayDetail")]
        public async Task<IActionResult> DeletePutAwayDetail([FromRoute] string putAwayCode, [FromQuery] string productCode)
        {
            try
            {
                var response = await _putAwayDetailService.DeletePutAwayDetail(putAwayCode, productCode);
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