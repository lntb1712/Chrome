using Chrome.DTO;
using Chrome.DTO.PutAwayDTO;
using Chrome.Services.PutAwayService;
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
    public class PutAwayController : ControllerBase
    {
        private readonly IPutAwayService _putAwayService;

        public PutAwayController(IPutAwayService putAwayService)
        {
            _putAwayService = putAwayService ?? throw new ArgumentNullException(nameof(putAwayService));
        }

        [HttpGet("GetAllPutAways")]
        public async Task<IActionResult> GetAllPutAways([FromQuery] string[] warehouseCodes, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _putAwayService.GetAllPutAwaysAsync(warehouseCodes, page, pageSize);
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

        [HttpGet("GetAllPutAwaysWithStatus")]
        public async Task<IActionResult> GetAllPutAwaysWithStatus([FromQuery] string[] warehouseCodes, [FromQuery] int statusId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _putAwayService.GetAllPutAwaysWithStatusAsync(warehouseCodes, statusId, page, pageSize);
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

        [HttpGet("SearchPutAways")]
        public async Task<IActionResult> SearchPutAways([FromQuery] string[] warehouseCodes, [FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _putAwayService.SearchPutAwaysAsync(warehouseCodes, textToSearch, page, pageSize);
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

        [HttpGet("GetPutAwayByCode")]
        public async Task<IActionResult> GetPutAwayByCode([FromQuery] string putAwayCode)
        {
            try
            {
                var response = await _putAwayService.GetPutAwayByCodeAsync(putAwayCode);
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

        [HttpPost("AddPutAway")]
        public async Task<IActionResult> AddPutAway([FromBody] PutAwayRequestDTO putAway)
        {
            try
            {
                var response = await _putAwayService.AddPutAway(putAway);
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

        [HttpDelete("DeletePutAway")]
        public async Task<IActionResult> DeletePutAway([FromQuery] string putAwayCode)
        {
            try
            {
                var response = await _putAwayService.DeletePutAway(putAwayCode);
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

        [HttpPut("UpdatePutAway")]
        public async Task<IActionResult> UpdatePutAway([FromBody] PutAwayRequestDTO putAway)
        {
            try
            {
                var response = await _putAwayService.UpdatePutAway(putAway);
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