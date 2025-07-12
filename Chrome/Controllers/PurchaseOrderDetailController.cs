using Chrome.DTO.PurchaseOrderDetailDTO;
using Chrome.Services.PurchaseOrderDetailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chrome.Controllers
{
    [Route("api/PurchaseOrder/{purchaseOrderCode}/[controller]")]
    [ApiController]
    [Authorize(Policy = "PermissionPolicy")]
    [EnableCors("MyCors")]
    public class PurchaseOrderDetailController : ControllerBase
    {
        private readonly IPurchaseOrderDetailService _purchaseOrderDetailService;
        public PurchaseOrderDetailController(IPurchaseOrderDetailService purchaseOrderDetailService)
        {
            _purchaseOrderDetailService = purchaseOrderDetailService;
        }
        [HttpGet("GetAllPurchaseOrderDetails")]
        public async Task<IActionResult> GetAllPurchaseOrderDetails([FromRoute] string purchaseOrderCode, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _purchaseOrderDetailService.GetAllPurchaseOrderDetails(purchaseOrderCode, page, pageSize);
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
        [HttpGet("GetPurchaseOrderDetailByCode")]
        public async Task<IActionResult> GetPurchaseOrderDetailByCode([FromRoute] string purchaseOrderCode, [FromQuery] string productCode)
        {
            try
            {
                var response = await _purchaseOrderDetailService.GetPurchaseOrderDetailByCode(purchaseOrderCode, productCode);
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

        [HttpPost("AddPurchaseOrderDetail")]
        public async Task<IActionResult> AddPurchaseOrderDetail([FromBody] PurchaseOrderDetailRequestDTO purchaseOrderDetail)
        {
            try
            {
                var response = await _purchaseOrderDetailService.AddPurchaseOrderDetail(purchaseOrderDetail);
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
        [HttpDelete("DeletePurchaseOrderDetail")]
        public async Task<IActionResult> DeletePurchaseOrderDetail([FromRoute] string purchaseOrderCode, [FromQuery] string productCode)
        {
            try
            {
                var response = await _purchaseOrderDetailService.DeletePurchaseOrderDetailAsync(purchaseOrderCode, productCode);
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
        [HttpPut("UpdatePurchaseOrderDetail")]
        public async Task<IActionResult> UpdatePurchaseOrderDetail([FromBody] PurchaseOrderDetailRequestDTO purchaseOrderDetail)
        {
            try
            {
                var response = await _purchaseOrderDetailService.UpdatePurchaseOrderDetail(purchaseOrderDetail);
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

        [HttpGet("GetListProductToPO")]
        public async Task<IActionResult> GetListProductToPO()
        {
            try
            {
                var response = await _purchaseOrderDetailService.GetListProductToPO();
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

        [HttpPost("ConfirmPurchaseOrderDetail")]
        public async Task<IActionResult> ConfirmPurchaseOrderDetail([FromRoute] string purchaseOrderCode)
        {
            try
            {
                var response = await _purchaseOrderDetailService.ConfirmPurchaseOrderDetail(purchaseOrderCode);
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

        [HttpPost("CreateBackOrder")]
        public async Task<IActionResult> CreateBackOrder([FromRoute] string purchaseOrderCode, [FromQuery] string backOrderDescription, [FromQuery] string dateBackOrder)
        {
            try
            {
                var response = await _purchaseOrderDetailService.CreateBackOrder(purchaseOrderCode, backOrderDescription, dateBackOrder);
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
        [HttpPost("CheckAndUpdateBackOrderStatus")]
        public async Task<IActionResult> CheckAndUpdateBackOrderStatus([FromRoute] string purchaseOrderCode)
        {
            try
            {
                var response = await _purchaseOrderDetailService.CheckAndUpdateBackOrderStatus(purchaseOrderCode);
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
