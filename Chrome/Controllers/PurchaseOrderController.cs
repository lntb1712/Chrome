using Chrome.DTO.PurchaseOrderDTO;
using Chrome.Services.PurchaseOrderService;
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
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        [HttpGet("GetAllPurchaseOrders")]
        public async Task<IActionResult> GetAllPurchaseOrders([FromQuery] string[] warehouseCodes, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _purchaseOrderService.GetAllPurchaseOrders(warehouseCodes, page, pageSize);
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
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi : {ex.Message}");
            }
        }

        [HttpGet("GetPurchaseOrderByCode")]
        public async Task<IActionResult> GetPurchaseOrderByCode([FromQuery] string purchaseOrderCode)
        {
            try
            {
                var response = await _purchaseOrderService.GetPurchaseOrderByCode(purchaseOrderCode);
                if (!response.Success)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = response.Message,
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("GetAllPurchaseOrdersWithStatus")]
        public async Task<IActionResult> GetAllPurchaseOrdersWithStatus([FromQuery] string[] warehouseCodes, [FromQuery] int statusId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _purchaseOrderService.GetAllPurchaseOrdersWithStatus(warehouseCodes, statusId, page, pageSize);
                if (!response.Success)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = response.Message,
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi :{ex.Message}");
            }
        }

        [HttpGet("SearchPurchaseOrderAsync")]
        public async Task<IActionResult> SearchPurchaseOrderAsync([FromQuery] string[] warehouseCodes, [FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _purchaseOrderService.SearchPurchaseOrderAsync(warehouseCodes, textToSearch, page, pageSize);
                if (!response.Success)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = response.Message,
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi : {ex.Message}");
            }
        }

        [HttpPost("AddPurchaseOrder")]
        public async Task<IActionResult> AddPurchaseOrder([FromBody] PurchaseOrderRequestDTO purchaseOrder)
        {
            try
            {
                var response = await _purchaseOrderService.AddPurchaseOrder(purchaseOrder);
                if (!response.Success)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = response.Message,
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi : {ex.Message}");
            }
        }

        [HttpDelete("DeletePurchaseOrderAsync")]
        public async Task<IActionResult> DeletePurchaseOrderAsync([FromQuery] string purchaseOrderCode)
        {
            try
            {
                var response = await _purchaseOrderService.DeletePurchaseOrderAsync(purchaseOrderCode);
                if (!response.Success)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = response.Message,
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi : {ex.Message}");
            }
        }

        [HttpPut("UpdatePurchaseOrder")]
        public async Task<IActionResult> UpdatePurchaseOrder([FromBody] PurchaseOrderRequestDTO purchaseOrder)
        {
            try
            {
                var response = await _purchaseOrderService.UpdatePurchaseOrder(purchaseOrder);
                if (!response.Success)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = response.Message,
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi : {ex.Message}");
            }
        }

        [HttpGet("GetListStatusMaster")]
        public async Task<IActionResult> GetListStatusMaster()
        {
            try
            {
                var response = await _purchaseOrderService.GetListStatusMaster();
                if (!response.Success)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = response.Message,
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi : {ex.Message}");
            }
        }

        [HttpGet("GetListSupplierMasterAsync")]
        public async Task<IActionResult> GetListSupplierMasterAsync()
        {
            try
            {
                var response = await _purchaseOrderService.GetListSupplierMasterAsync();
                if (!response.Success)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = response.Message,
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi : {ex.Message}");
            }
        }

        [HttpGet("GetListWarehousePermission")]
        public async Task<IActionResult> GetListWarehousePermission([FromQuery] string[] warehouseCodes)
        {
            try
            {
                var response = await _purchaseOrderService.GetListWarehousePermission(warehouseCodes);
                if (!response.Success)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = response.Message,
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi : {ex.Message}");
            }


        }
    }
}
