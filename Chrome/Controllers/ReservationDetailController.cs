using Chrome.DTO;
using Chrome.DTO.ReservationDetailDTO;
using Chrome.Services.ReservationDetailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chrome.Controllers
{
    [Route("api/Reservation/{reservationCode}/[controller]")]
    [ApiController]
    [Authorize(Policy = "PermissionPolicy")]
    [EnableCors("MyCors")]
    public class ReservationDetailController : ControllerBase
    {
        private readonly IReservationDetailService _reservationDetailService;

        public ReservationDetailController(IReservationDetailService reservationDetailService)
        {
            _reservationDetailService = reservationDetailService ?? throw new ArgumentNullException(nameof(reservationDetailService));
        }

        [HttpGet("GetAllReservationDetails")]
        public async Task<IActionResult> GetAllReservationDetails([FromRoute] string reservationCode, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(reservationCode))
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Mã reservation không được để trống"
                    });
                }
                string decodedResCode = Uri.UnescapeDataString(reservationCode);
                var response = await _reservationDetailService.GetAllReservationDetails(decodedResCode, page, pageSize);
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

        [HttpGet("GetReservationDetailById")]
        public async Task<IActionResult> GetReservationDetailById([FromQuery] int id)
        {
            try
            {
                var response = await _reservationDetailService.GetReservationDetailByIdAsync(id);
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

        [HttpGet("SearchReservationDetails")]
        public async Task<IActionResult> SearchReservationDetails([FromRoute] string reservationCode, [FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(reservationCode))
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Mã reservation không được để trống"
                    });
                }
                string decodedResCode = Uri.UnescapeDataString(reservationCode);
                var response = await _reservationDetailService.SearchReservationDetailsAsync(decodedResCode, textToSearch, page, pageSize);
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