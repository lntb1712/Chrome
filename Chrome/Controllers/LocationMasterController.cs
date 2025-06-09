using Chrome.DTO.LocationMasterDTO;
using Chrome.Services.LocationMasterService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chrome.Controllers
{
    [Route("api/WarehouseMaster/{warehouseCode}/[controller]")]
    [ApiController]
    [Authorize(Policy = "PermissionPolicy")]
    [EnableCors("MyCors")]

    public class LocationMasterController : ControllerBase
    {
        private readonly ILocationMasterService _locationMasterService;
        public LocationMasterController(ILocationMasterService locationMasterService)
        {
            _locationMasterService = locationMasterService;
        }
        [HttpGet("GetAllLocationMaster")]
        public async Task<IActionResult> GetAllLocationMaster([FromRoute]string warehouseCode,[FromQuery] int page=1, [FromQuery] int pageSize=10)
        {
            try
            {
                var response = await _locationMasterService.GetAllLocationMaster(warehouseCode,page, pageSize);
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
        [HttpGet("GetLocationMasterWithCode")]
        public async Task<IActionResult> GetLocationMasterWithCode([FromRoute] string warehouseCode, [FromQuery] string locationCode)
        {
            try
            {
                var response = await _locationMasterService.GetLocationMasterWithCode(warehouseCode, locationCode);
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
        [HttpGet("GetTotalLocationMasterCount")]
        public async Task<IActionResult> GetTotalLocationMasterCount([FromRoute] string warehouseCode)
        {
            try
            {
                var response = await _locationMasterService.GetTotalLocationMasterCount(warehouseCode);
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
        [HttpPost("AddLocationMaster")]
        public async Task<IActionResult> AddLocationMaster([FromBody] LocationMasterRequestDTO locationMasterRequestDTO)
        {
            try
            {
                var response = await _locationMasterService.AddLocationMaster(locationMasterRequestDTO);
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
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }
        [HttpDelete("DeleteLocationMaster")]
        public async Task<IActionResult> DeleteLocationMaster([FromRoute]string warehouseCode,[FromQuery] string locationCode)
        {
            try
            {
                var response = await _locationMasterService.DeleteLocationMaster(warehouseCode, locationCode);
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
        [HttpPut("UpdateLocationMaster")]
        public async Task<IActionResult> UpdateLocationMaster([FromBody] LocationMasterRequestDTO locationMasterRequestDTO)
        {
            try
            {
                var response = await _locationMasterService.UpdateLocationMaster(locationMasterRequestDTO);
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
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }
    }
}
