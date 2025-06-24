using Chrome.DTO;
using Chrome.DTO.PickListDTO;
using Chrome.Services.PickListService;
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
    public class PickListController : ControllerBase
    {
        private readonly IPickListService _pickListService;

        public PickListController(IPickListService pickListService)
        {
            _pickListService = pickListService ?? throw new ArgumentNullException(nameof(pickListService));
        }

        [HttpGet("GetAllPickLists")]
        public async Task<IActionResult> GetAllPickLists([FromQuery] string[] warehouseCodes, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _pickListService.GetAllPickListsAsync(warehouseCodes, page, pageSize);
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

        [HttpGet("GetAllPickListsWithStatus")]
        public async Task<IActionResult> GetAllPickListsWithStatus([FromQuery] string[] warehouseCodes, [FromQuery] int statusId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _pickListService.GetAllPickListsWithStatusAsync(warehouseCodes, statusId, page, pageSize);
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

        [HttpGet("SearchPickLists")]
        public async Task<IActionResult> SearchPickLists([FromQuery] string[] warehouseCodes, [FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _pickListService.SearchPickListsAsync(warehouseCodes, textToSearch, page, pageSize);
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

        [HttpGet("GetPickListByCode")]
        public async Task<IActionResult> GetPickListByCode([FromQuery] string pickNo)
        {
            try
            {
                string decodedPickNo = Uri.UnescapeDataString(pickNo);
                var response = await _pickListService.GetPickListByCodeAsync(decodedPickNo);
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

        [HttpPost("AddPickList")]
        public async Task<IActionResult> AddPickList([FromBody] PickListRequestDTO pickList)
        {
            try
            {
                var response = await _pickListService.AddPickList(pickList);
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

        [HttpDelete("DeletePickList")]
        public async Task<IActionResult> DeletePickList([FromQuery] string pickNo)
        {
            try
            {
                string decodedPickNo = Uri.UnescapeDataString(pickNo);
                var response = await _pickListService.DeletePickList(decodedPickNo);
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

        [HttpPut("UpdatePickList")]
        public async Task<IActionResult> UpdatePickList([FromBody] PickListRequestDTO pickList)
        {
            try
            {
                var response = await _pickListService.UpdatePickList(pickList);
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
                var response = await _pickListService.GetListStatusMaster();
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
        [HttpGet("GetPickListByStockOutCodeAsync")]
        public async Task<IActionResult> GetPickListByStockOutCodeAsync([FromQuery]string stockOutCode)
        {
            try
            {
                string decodedStockOutCode = Uri.UnescapeDataString(stockOutCode);
                var response = await _pickListService.GetPickListByStockOutCodeAsync(decodedStockOutCode);
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