using Chrome.DTO.BOMComponentDTO;
using Chrome.Services.BOMComponentService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chrome.Controllers
{
    [Route("api/BOMMaster/{bomCode}/{bomVersion}/[controller]")]
    [ApiController]
    public class BOMComponentController : ControllerBase
    {
        private readonly IBOMComponentService _bomComponentService;
        public BOMComponentController(IBOMComponentService bomComponentService)
        {
            _bomComponentService = bomComponentService;
        }

        [HttpGet("GetRecursiveBOMAsync")]
        public async Task<IActionResult> GetRecursiveBOMAsync([FromRoute] string bomCode, [FromRoute] string bomVersion)
        {
            try
            {
                var response = await _bomComponentService.GetRecursiveBOMAsync(bomCode, bomVersion);
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
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("GetAllBOMComponent")]
        public async Task<IActionResult> GetAllBOMComponent([FromRoute] string bomCode, [FromRoute] string bomVersion)
        {
            try
            {
                var response = await _bomComponentService.GetAllBOMComponent(bomCode, bomVersion);
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
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi : {ex.Message}");
            }
        }

        [HttpGet("GetBOMComponent")]
        public async Task<IActionResult> GetBOMComponent([FromRoute] string bomCode, [FromQuery] string componentCode, [FromRoute] string bomVersion)
        {
            try
            {
                var response = await _bomComponentService.GetBOMComponent(bomCode, componentCode, bomVersion);
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

        [HttpPost("AddBomComponent")]
        public async Task<IActionResult> AddBomComponent([FromBody] BOMComponentRequestDTO bomComponent)
        {
            try
            {
                var response = await _bomComponentService.AddBomComponent(bomComponent);
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

        [HttpPut("UpdateBomComponent")]
        public async Task<IActionResult> UpdateBomComponent([FromBody] BOMComponentRequestDTO bomComponent)
        {
            try
            {
                var response = await _bomComponentService.UpdateBomComponent(bomComponent);
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

        [HttpDelete("DeleteBomComponent")]
        public async Task<IActionResult> DeleteBomComponent([FromRoute] string bomCode, [FromQuery] string componentCode, [FromRoute] string bomVersion)
        {
            try
            {
                var response = await _bomComponentService.DeleteBomComponent(bomCode, componentCode, bomVersion);
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
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi : {ex.Message}");
            }
        }
    }
}
