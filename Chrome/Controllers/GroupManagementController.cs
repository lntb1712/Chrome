using Chrome.DTO.GroupManagementDTO;
using Chrome.Services.GroupFunctionService;
using Chrome.Services.GroupManagementService;
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
    public class GroupManagementController : ControllerBase
    {
        private readonly IGroupManagementService _groupManagementService;
        private readonly IGroupFunctionService _groupFunctionService;

        public GroupManagementController(IGroupManagementService groupManagementService, IGroupFunctionService groupFunctionService)
        {
            _groupManagementService = groupManagementService;
            _groupFunctionService = groupFunctionService;
        }

        [HttpPost("AddGroupManagement")]
        public async Task<IActionResult> AddGroupManagement([FromBody] GroupManagementRequestDTO groupRequest)
        {
            if (groupRequest == null)
            {
                return BadRequest("Dữ liệu nhận vào không hợp lệ");
            }

            try
            {
                var response = await _groupManagementService.AddGroupManagement(groupRequest);
                if (response.Success == false)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = response.Message
                    });
                }
                return Ok(new { Success = true, Message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi{ex.Message}");
            }
        }

        [HttpGet("GetAllGroupManagement")]
        public async Task<IActionResult> GetAllGroupManagement([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _groupManagementService.GetAllGroupManagement(page, pageSize);
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

        [HttpGet("GetAllGroupFunctions")]
        public async Task<IActionResult> GetAllGroupFunctions()
        {
            try
            {
                var response = await _groupFunctionService.GetAllGroupFunctions();
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

        [HttpGet("GetGroupFunctionWithGroupID")]
        public async Task<IActionResult> GetGroupFunctionWithGroupID([FromQuery] string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                return BadRequest("Dữ liệu nhận vào không hợp lệ");
            }
            try
            {
                var response = await _groupFunctionService.GetGroupFunctionWithGroupID(groupId);
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

        [HttpDelete("DeleteGroupManagement")]
        public async Task<IActionResult> DeleteGroupManagement([FromQuery] string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                return BadRequest("Dữ liệu nhận vào không hợp lệ");
            }
            try
            {
                var lst = await _groupFunctionService.GetGroupFunctionWithGroupID(groupId);
                if (lst.Data!=null)
                {
                    foreach (var item in lst.Data)
                    {
                        await _groupFunctionService.DeleteGroupFunction(groupId, item.FunctionId);
                    }
                }
                var response = await _groupManagementService.DeleteGroupManagement(groupId);
                if (response.Success == false)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = response.Message
                    });
                }
                return Ok(new { Success = true, Message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi {ex.Message}");
            }
        }

        [HttpPut("UpdateGroupManagement")]
        public async Task<IActionResult> UpdateGroupManagement([FromBody] GroupManagementRequestDTO groupManagementRequestDTO)
        {
            if (groupManagementRequestDTO == null)
            {
                return BadRequest("Dữ liệu nhận vào không hợp lệ");
            }
            try
            {
                var response = await _groupManagementService.UpdateGroupManagement(groupManagementRequestDTO);
                if (response.Success == false)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = response.Message
                    });
                }
                return Ok(new { Success = true, Message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi {ex.Message}");
            }
        }

        [HttpGet("SearchGroupInList")]
        public async Task<IActionResult> SearchGroupInList([FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _groupManagementService.SearchGroup(textToSearch, page, pageSize);
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
        
        [HttpGet("GetGroupManagemetWithGroupId")]
        public async Task<IActionResult> GetGroupManagemetWithGroupId([FromQuery] string groupId)
        {
            try
            {
                var response = await _groupManagementService.GetGroupManagementWithGroupId(groupId);
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

        [HttpGet("GetTotalUserInGroup")]
        public async Task<IActionResult> GetTotalUserInGroup()
        {
            try
            {
                var response = await _groupManagementService.GetTotalUserInGroup();
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
        [HttpGet("GetTotalGroupCount")]
        public async Task<IActionResult> GetTotalGroupCount()
        {
            try
            {
                var response = await _groupManagementService.GetTotalGroupCount();
                if(!response.Success)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = response.Message
                    }); 
                }
                return Ok(response);
            }catch(Exception ex)
            {
                return StatusCode(500, "Lỗi:" + ex.Message);
            }
        }
    }
}
