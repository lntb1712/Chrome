using Chrome.DTO.ManufacturingOrderDTO;
using Chrome.Services.ManufacturingOrderService;
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
    public class ManufacturingOrderController : ControllerBase
    {
        private readonly IManufacturingOrderService _manufacturingOrderService;

        public ManufacturingOrderController(IManufacturingOrderService manufacturingOrderService)
        {
            _manufacturingOrderService = manufacturingOrderService;
        }

        [HttpPost("AddManufacturingOrder")]
        public async Task<IActionResult> AddManufacturingOrder([FromBody] ManufacturingOrderRequestDTO manufacturingOrderRequest)
        {
            try
            {
                var response = await _manufacturingOrderService.AddManufacturingOrderAsync(manufacturingOrderRequest);
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

        [HttpGet("GetAllManufacturingOrders")]
        public async Task<IActionResult> GetAllManufacturingOrders([FromQuery] string[] warehouseCodes, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _manufacturingOrderService.GetAllManufacturingOrdersAsync(warehouseCodes, page, pageSize);
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

        [HttpGet("GetAllManufacturingOrdersAsyncWithResponsible")]
        public async Task<IActionResult> GetAllManufacturingOrdersAsyncWithResponsible([FromQuery] string[] warehouseCodes,[FromQuery]string responsible, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _manufacturingOrderService.GetAllManufacturingOrdersAsyncWithResponsible(warehouseCodes,responsible, page, pageSize);
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

        [HttpGet("GetAllManufacturingOrdersWithStatus")]
        public async Task<IActionResult> GetAllManufacturingOrdersWithStatus([FromQuery] string[] warehouseCodes, [FromQuery] int statusId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _manufacturingOrderService.GetAllManufacturingOrdersWithStatusAsync(warehouseCodes, statusId, page, pageSize);
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

        [HttpGet("GetManufacturingOrderByCode")]
        public async Task<IActionResult> GetManufacturingOrderByCode([FromQuery] string manufacturingCode)
        {
            try
            {
                string decodedManufacturingOrderCode = Uri.UnescapeDataString(manufacturingCode);
                var response = await _manufacturingOrderService.GetManufacturingOrderByCodeAsync(decodedManufacturingOrderCode);
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

        [HttpGet("SearchManufacturingOrders")]
        public async Task<IActionResult> SearchManufacturingOrders([FromQuery] string[] warehouseCodes, [FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _manufacturingOrderService.SearchManufacturingOrdersAsync(warehouseCodes, textToSearch, page, pageSize);
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

        [HttpGet("SearchManufacturingOrdersAsyncWithResponsible")]
        public async Task<IActionResult> SearchManufacturingOrdersAsyncWithResponsible([FromQuery] string[] warehouseCodes,[FromQuery] string responsible, [FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _manufacturingOrderService.SearchManufacturingOrdersAsyncWithResponsible(warehouseCodes,responsible, textToSearch, page, pageSize);
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

        [HttpPut("UpdateManufacturingOrder")]
        public async Task<IActionResult> UpdateManufacturingOrder([FromBody] ManufacturingOrderRequestDTO manufacturingOrderRequest)
        {
            try
            {
                var response = await _manufacturingOrderService.UpdateManufacturingOrderAsync(manufacturingOrderRequest);
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

        [HttpDelete("DeleteManufacturingOrder")]
        public async Task<IActionResult> DeleteManufacturingOrder([FromQuery] string manufacturingCode)
        {
            try
            {
                string decodedManufacturingOrderCode = Uri.UnescapeDataString(manufacturingCode);
                var response = await _manufacturingOrderService.DeleteManufacturingOrderAsync(decodedManufacturingOrderCode);
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

        [HttpPost("ConfirmManufacturingOrder")]
        public async Task<IActionResult> ConfirmManufacturingOrder([FromQuery] string manufacturingCode)
        {
            try
            {
                string decodedManufacturingOrderCode = Uri.UnescapeDataString(manufacturingCode);
                var response = await _manufacturingOrderService.ConfirmManufacturingOrder(decodedManufacturingOrderCode);
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

        [HttpPost("CreateBackOrder")]
        public async Task<IActionResult> CreateBackOrder([FromQuery] string manufacturingCode, [FromQuery] string scheduleDateBackOrder, [FromQuery]string deadLineBackOrder)
        {
            try
            {
                string decodedManufacturingOrderCode = Uri.UnescapeDataString(manufacturingCode);
                var response = await _manufacturingOrderService.CreateBackOrder(decodedManufacturingOrderCode, scheduleDateBackOrder,deadLineBackOrder);
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

        [HttpPost("CheckAndUpdateBackOrderStatus")]
        public async Task<IActionResult> CheckAndUpdateBackOrderStatus([FromQuery] string manufacturingCode)
        {
            try
            {
                string decodedManufacturingOrderCode = Uri.UnescapeDataString(manufacturingCode);
                var response = await _manufacturingOrderService.CheckAndUpdateBackOrderStatus(decodedManufacturingOrderCode);
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

        [HttpGet("GetListOrderType")]
        public async Task<IActionResult> GetListOrderType([FromQuery] string prefix)
        {
            try
            {
                var response = await _manufacturingOrderService.GetListOrderTypeAsync(prefix);
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

        [HttpGet("GetListResponsible")]
        public async Task<IActionResult> GetListResponsible([FromQuery] string warehouseCode)
        {
            try
            {
                var response = await _manufacturingOrderService.GetListResponsibleAsync(warehouseCode);
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

        [HttpGet("GetListStatusMaster")]
        public async Task<IActionResult> GetListStatusMaster()
        {
            try
            {
                var response = await _manufacturingOrderService.GetListStatusMasterAsync();
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

        [HttpGet("GetListWarehousePermission")]
        public async Task<IActionResult> GetListWarehousePermission([FromQuery] string[] warehouseCodes)
        {
            try
            {
                var response = await _manufacturingOrderService.GetListWarehousePermissionAsync(warehouseCodes);
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
        [HttpGet("GetListProductMasterIsFGAndSFG")]
        public async Task<IActionResult> GetListProductMasterIsFGAndSFG()
        {
            try
            {
                var response = await _manufacturingOrderService.GetListProductMasterIsFGAndSFG();
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

        [HttpGet("GetListBomMaster")]
        public async Task<IActionResult> GetListBomMaster([FromQuery]string productCode)
        {
            try
            {
                var response = await _manufacturingOrderService.GetListBomMasterAsync(productCode);
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

        [HttpPost("CheckInventory")]
        public async Task<IActionResult> CheckInventory([FromBody] ManufacturingOrderRequestDTO manufacturingOrder)
        {
            try
            {
                var response = await _manufacturingOrderService.CheckInventory(manufacturingOrder);
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
            catch( Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi : {ex.Message}");
            }
        }
        [HttpPost("CheckQuantityWithBase")]
        public async Task<IActionResult> CheckQuantityWithBase([FromBody] ManufacturingOrderRequestDTO manufacturingOrder)
        {
            try
            {
                var response = await _manufacturingOrderService.CheckQuantityWithBase(manufacturingOrder);
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
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi :{ex.Message}");
            }
        }


    }
}