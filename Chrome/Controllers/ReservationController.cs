using Chrome.DTO;
using Chrome.DTO.OrderTypeDTO;
using Chrome.DTO.ReservationDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.DTO.WarehouseMasterDTO;
using Chrome.Services.ReservationService;
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
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService ?? throw new ArgumentNullException(nameof(reservationService));
        }

        [HttpGet("GetAllReservations")]
        public async Task<IActionResult> GetAllReservations([FromQuery] string[] warehouseCodes, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _reservationService.GetAllReservations(warehouseCodes, page, pageSize);
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

        [HttpGet("GetAllReservationsWithStatus")]
        public async Task<IActionResult> GetAllReservationsWithStatus([FromQuery] string[] warehouseCodes, [FromQuery] int statusId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _reservationService.GetAllReservationsWithStatus(warehouseCodes, statusId, page, pageSize);
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

        [HttpGet("SearchReservationsAsync")]
        public async Task<IActionResult> SearchReservationsAsync([FromQuery] string[] warehouseCodes, [FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _reservationService.SearchReservationsAsync(warehouseCodes, textToSearch, page, pageSize);
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

        [HttpGet("GetListOrderType")]
        public async Task<IActionResult> GetListOrderType([FromQuery] string prefix)
        {
            try
            {
                var response = await _reservationService.GetListOrderType(prefix);
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

        [HttpGet("GetListStatusMaster")]
        public async Task<IActionResult> GetListStatusMaster()
        {
            try
            {
                var response = await _reservationService.GetListStatusMaster();
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

        [HttpGet("GetListWarehousePermission")]
        public async Task<IActionResult> GetListWarehousePermission([FromQuery] string[] warehouseCodes)
        {
            try
            {
                var response = await _reservationService.GetListWarehousePermission(warehouseCodes);
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

        [HttpGet("GetReservationsByStockOutCodeAsync")]
        public async Task<IActionResult> GetReservationsByStockOutCodeAsync([FromQuery] string stockOutCode)
        {
            try
            {
                var response = await _reservationService.GetReservationsByStockOutCodeAsync(stockOutCode);
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
        [HttpGet("GetReservationsByTransferCodeAsync")]
        public async Task<IActionResult> GetReservationsByTransferCodeAsync([FromQuery] string transferCode)
        {
            try
            {
                var response = await _reservationService.GetReservationsByStockOutCodeAsync(transferCode);
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
        [HttpGet("GetReservationsByMovementCodeAsync")]
        public async Task<IActionResult> GetReservationsByMovementCodeAsync([FromQuery] string movementCode)
        {
            try
            {
                var response = await _reservationService.GetReservationsByStockOutCodeAsync(movementCode);
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
        [HttpGet("GetReservationsByManufacturingCodeAsync")]
        public async Task<IActionResult> GetReservationsByManufacturingCodeAsync([FromQuery] string manufacturingCode)
        {
            try
            {
                var response = await _reservationService.GetReservationsByManufacturingCodeAsync(manufacturingCode);
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

        [HttpPost("AddReservation")]
        public async Task<IActionResult> AddReservation([FromBody] ReservationRequestDTO reservation)
        {
            try
            {
                if (reservation == null)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    });
                }

                var response = await _reservationService.AddOrUpdateReservation(reservation);
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

        [HttpDelete("DeleteReservationAsync")]
        public async Task<IActionResult> DeleteReservationAsync([FromQuery] string reservationCode)
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

                var response = await _reservationService.DeleteReservationAsync(reservationCode);
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