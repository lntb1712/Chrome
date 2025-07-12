using Chrome.DTO;
using Chrome.DTO.ManufacturingOrderDetailDTO;
using Chrome.Services.ManufacturingOrderDetailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Chrome.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "PermissionPolicy")]
    [EnableCors("MyCors")]
    public class ManufacturingOrderDetailController : ControllerBase
    {
        private readonly IManufacturingOrderDetailService _manufacturingOrderDetailService;

        public ManufacturingOrderDetailController(IManufacturingOrderDetailService manufacturingOrderDetailService)
        {
            _manufacturingOrderDetailService = manufacturingOrderDetailService;
        }

        [HttpGet("GetManufacturingOrderDetails")]
        public async Task<IActionResult> GetManufacturingOrderDetails([FromQuery] string manufacturingOrderCode)
        {
            try
            {
                var response = await _manufacturingOrderDetailService.GetManufacturingOrderDetail(manufacturingOrderCode);
                if (!response.Success)
                {
                    return BadRequest(new { Success = false, Message = response.Message });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = $"Lỗi: {ex.Message}" });
            }
        }

        [HttpGet("GetManufacturingOrderDetailByCode")]
        public async Task<IActionResult> GetManufacturingOrderDetailByCode([FromQuery] string manufacturingOrderCode, [FromQuery] string productCode)
        {
            try
            {
                var response = await _manufacturingOrderDetailService.GetManufacturingOrderDetail(manufacturingOrderCode, productCode);
                if (!response.Success)
                {
                    return NotFound(new { Success = false, Message = response.Message });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = $"Lỗi: {ex.Message}" });
            }
        }
        [HttpGet("GetForecastManufacturingOrderDetail")]
        public async Task<IActionResult> GetForecastManufacturingOrderDetail([FromQuery] string manufacturingOrderCode, [FromQuery] string productCode)
        {
            try
            {
                var response = await _manufacturingOrderDetailService.GetForecastManufacturingOrderDetail(manufacturingOrderCode, productCode);
                if (!response.Success)
                {
                    return NotFound(new { Success = false, Message = response.Message });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = $"Lỗi: {ex.Message}" });
            }
        }

        [HttpPut("UpdateListManufacturingOrderDetail")]
        public async Task<IActionResult> UpdateListManufacturingOrderDetail([FromBody] List<ManufacturingOrderDetailRequestDTO> detailRequestDTOs)
        {
            try
            {
                var response = await _manufacturingOrderDetailService.UpdateListManufacturingOrderDetail(detailRequestDTOs);
                if (!response.Success)
                {
                    return Conflict(new { Success = false, Message = response.Message });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = $"Lỗi: {ex.Message}" });
            }
        }
    }
}