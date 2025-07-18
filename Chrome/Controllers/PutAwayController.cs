using Chrome.DTO.PutAwayDTO;
using Chrome.Services.PutAwayService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
        [HttpGet("GetAllPutAwaysAsyncWithResponsible")]
        public async Task<IActionResult> GetAllPutAwaysAsyncWithResponsible([FromQuery] string[] warehouseCodes,[FromQuery]string responsible, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _putAwayService.GetAllPutAwaysAsyncWithResponsible(warehouseCodes,responsible, page, pageSize);
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
        [HttpGet("SearchPutAwaysAsyncWithResponsible")]
        public async Task<IActionResult> SearchPutAwaysAsyncWithResponsible([FromQuery] string[] warehouseCodes,[FromQuery]string responsible, [FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _putAwayService.SearchPutAwaysAsyncWithResponsible(warehouseCodes,responsible, textToSearch, page, pageSize);
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
                string decodedPutAwayCode = Uri.UnescapeDataString(putAwayCode);
                var response = await _putAwayService.GetPutAwayByCodeAsync(decodedPutAwayCode);
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
        [HttpGet("GetPutAwayContainsCodeAsync")]
        public async Task<IActionResult> GetPutAwayContainsCodeAsync([FromQuery] string orderCode)
        {
            try
            {
                string decodedPutAwayCode = Uri.UnescapeDataString(orderCode);
                var response = await _putAwayService.GetPutAwayContainsCodeAsync(decodedPutAwayCode);
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
        [HttpGet("GetListPutAwayContainsCodeAsync")]
        public async Task<IActionResult> GetListPutAwayContainsCodeAsync([FromQuery] string orderCode)
        {
            try
            {
                string decodedPutAwayCode = Uri.UnescapeDataString(orderCode);
                var response = await _putAwayService.GetListPutAwayContainsCodeAsync(decodedPutAwayCode);
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
                string decodedPutAwayCode = Uri.UnescapeDataString(putAwayCode);
                var response = await _putAwayService.DeletePutAway(decodedPutAwayCode);
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
        [HttpGet("GetListStatusMaster")]
        public async Task<IActionResult> GetListStatusMaster()
        {
            try
            {
                var response = await _putAwayService.GetListStatusMaster();
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