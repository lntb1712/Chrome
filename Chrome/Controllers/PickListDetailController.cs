using Chrome.DTO;
using Chrome.DTO.PickListDetailDTO;
using Chrome.Services.PickListDetailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chrome.Controllers
{
    [Route("api/PickList/{pickNo}/[controller]")]
    [ApiController]
    [Authorize(Policy = "PermissionPolicy")]
    [EnableCors("MyCors")]
    public class PickListDetailController : ControllerBase
    {
        private readonly IPickListDetailService _pickListDetailService;

        public PickListDetailController(IPickListDetailService pickListDetailService)
        {
            _pickListDetailService = pickListDetailService ?? throw new ArgumentNullException(nameof(pickListDetailService));
        }

        [HttpGet("GetAllPickListDetails")]
        public async Task<IActionResult> GetAllPickListDetails([FromQuery] string[] warehouseCodes, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _pickListDetailService.GetAllPickListDetailsAsync(warehouseCodes, page, pageSize);
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

        [HttpGet("GetPickListDetailsByPickNo")]
        public async Task<IActionResult> GetPickListDetailsByPickNo([FromRoute] string pickNo, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _pickListDetailService.GetPickListDetailsByPickNoAsync(pickNo, page, pageSize);
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

        [HttpGet("SearchPickListDetails")]
        public async Task<IActionResult> SearchPickListDetails([FromQuery] string[] warehouseCodes,[FromRoute]string pickNo, [FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _pickListDetailService.SearchPickListDetailsAsync(warehouseCodes,pickNo, textToSearch, page, pageSize);
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

        [HttpPut("UpdatePickListDetail")]
        public async Task<IActionResult> UpdatePickListDetail([FromBody] PickListDetailRequestDTO pickListDetail)
        {
            try
            {
                var response = await _pickListDetailService.UpdatePickListDetail(pickListDetail);
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

        [HttpDelete("DeletePickListDetail")]
        public async Task<IActionResult> DeletePickListDetail([FromRoute] string pickNo, [FromQuery] string productCode)
        {
            try
            {
                var response = await _pickListDetailService.DeletePickListDetail(pickNo, productCode);
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