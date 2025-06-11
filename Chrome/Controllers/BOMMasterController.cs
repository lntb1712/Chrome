using Chrome.DTO.BOMMasterDTO;
using Chrome.Services.BOMMasterService;
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
    public class BOMMasterController : ControllerBase
    {
        private readonly IBOMMasterService _bomMasterService;
        public BOMMasterController(IBOMMasterService bomMasterService)
        {
            _bomMasterService = bomMasterService;
        }
        [HttpGet("GetAllBOMMaster")]
        public async Task<IActionResult> GetAllBOMMaster([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _bomMasterService.GetAllBOMMaster(page, pageSize);
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
                return StatusCode(500, $"Lỗi {ex.Message}");
            }
        }

        [HttpGet("GetBOMMasterByCode")]
        public async Task<IActionResult> GetBOMMasterByCode([FromQuery] string bomCode, [FromQuery] string bomVersion)
        {
            try
            {
                var response = await _bomMasterService.GetBOMMasterByCode(bomCode, bomVersion);
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
                return StatusCode(500, $"Lỗi :{ex.Message}");
            }
        }

        [HttpGet("GetTotalBOMMasterCount")]
        public async Task<IActionResult> GetTotalBOMMasterCount()
        {
            try
            {
                var response = await _bomMasterService.GetTotalBOMMasterCount();
                if (!response.Success)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = response.Message
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi :{ex.Message}");
            }
        }

        [HttpGet("SearchBOMMaster")]
        public async Task<IActionResult> SearchBOMMaster([FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _bomMasterService.SearchBOMMaster(textToSearch, page, pageSize);
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
                return StatusCode(500, $"Lỗi : {ex.Message}");
            }
        }

        [HttpPost("AddBOMMaster")]
        public async Task<IActionResult> AddBOMMaster([FromBody] BOMMasterRequestDTO bomMaster)
        {
            try
            {
                var response = await _bomMasterService.AddBOMMaster(bomMaster);
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
                return StatusCode(500, $"Lỗi :{ex.Message}");
            }
        }

        [HttpDelete("DeleteBOMMaster")]
        public async Task<IActionResult> DeleteBOMMaster([FromQuery] string bomCode, [FromQuery] string bomVersion)
        {
            try
            {
                var response = await _bomMasterService.DeleteBOMMaster(bomCode, bomVersion);
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
                return StatusCode(500, $"Lỗi :{ex.Message}");
            }
        }

        [HttpPut("UpdateBOMMaster")]
        public async Task<IActionResult> UpdateBOMMaster([FromBody] BOMMasterRequestDTO bomMaster)
        {
            try
            {
                var response = await _bomMasterService.UpdateBOMMaster(bomMaster);
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
            catch(Exception ex)
            {
                return StatusCode(500, $"Lỗi : {ex.Message}");
            }
        }
    }
}
