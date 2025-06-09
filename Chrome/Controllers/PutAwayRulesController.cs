using Chrome.DTO.PutAwayRulesDTO;
using Chrome.Services.PutAwayRulesService;
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
    public class PutAwayRulesController : ControllerBase
    {
        private readonly IPutAwayRulesService _putAwayRulesService;
        public PutAwayRulesController(IPutAwayRulesService putAwayRulesService)
        {
            _putAwayRulesService = putAwayRulesService;
        }
        [HttpGet("GetAllPutAwayRules")]
        public async Task<IActionResult> GetAllPutAwayRules([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _putAwayRulesService.GetAllPutAwayRules(page, pageSize);
                if (!response.Success)
                {
                    return NotFound(new { Success = false, Message = response.Message });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
        [HttpGet("GetPutAwayRuleWithCode")]
        public async Task<IActionResult> GetPutAwayRuleWithCode([FromQuery] string putAwayRuleCode)
        {
            try
            {
                var response = await _putAwayRulesService.GetPutAwayRuleWithCode(putAwayRuleCode);
                if (!response.Success)
                {
                    return NotFound(new { Success = false, Message = response.Message });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [HttpGet("GetTotalPutAwayRuleCount")]
        public async Task<IActionResult> GetTotalPutAwayRuleCount()
        {
            try
            {
                var response = await _putAwayRulesService.GetTotalPutAwayRuleCount();
                if (!response.Success)
                {
                    return NotFound(new { Success = false, Message = response.Message });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
        [HttpGet("SearchPutAwayRules")]
        public async Task<IActionResult> SearchPutAwayRules([FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _putAwayRulesService.SearchPutAwayRules(textToSearch, page, pageSize);
                if (!response.Success)
                {
                    return NotFound(new { Success = false, Message = response.Message });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [HttpPost("AddPutAwayRule")]
        public async Task<IActionResult> AddPutAwayRule([FromBody] PutAwayRulesRequestDTO putAwayRuleDTO)
        {
            try
            {
                var response = await _putAwayRulesService.AddPutAwayRule(putAwayRuleDTO);
                if (!response.Success)
                {
                    return BadRequest(new { Success = false, Message = response.Message });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
        [HttpPut("UpdatePutAwayRule")]
        public async Task<IActionResult> UpdatePutAwayRule([FromBody] PutAwayRulesRequestDTO putAwayRuleDTO)
        {
            try
            {
                var response = await _putAwayRulesService.UpdatePutAwayRule(putAwayRuleDTO);
                if (!response.Success)
                {
                    return BadRequest(new { Success = false, Message = response.Message });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
        [HttpDelete("DeletePutAwayRule")]
        public async Task<IActionResult> DeletePutAwayRule([FromQuery] string putAwayRuleCode)
        {
            try
            {
                var response = await _putAwayRulesService.DeletePutAwayRule(putAwayRuleCode);
                if (!response.Success)
                {
                    return NotFound(new { Success = false, Message = response.Message });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }

        }
    }
}
