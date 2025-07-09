using Chrome.DTO.DashboardDTO;
using Chrome.Services.DashboardService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chrome.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("GetDashboardInformation")]
        public async Task<IActionResult> GetDashboardInformation([FromQuery] DashboardRequestDTO dashboardRequest)
        {
            try
            {
                var response = await _dashboardService.GetDashboardInformation(dashboardRequest);
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
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [HttpGet("GetStockInOutSummaryAsync")]
        public async Task<IActionResult> GetStockInOutSummaryAsync([FromQuery] DashboardRequestDTO dashboardRequest)
        {
            try
            {
                var response = await _dashboardService.GetStockInOutSummaryAsync(dashboardRequest);
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
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
    }
}
