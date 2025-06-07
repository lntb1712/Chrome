using Chrome.DTO.ProductCustomerDTO;
using Chrome.Services.ProductCustomerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chrome.Controllers
{
    [Route("/api/ProductMaster/{productCode}/[controller]")]
    [ApiController]
    [Authorize(Policy = "PermissionPolicy")]
    [EnableCors("MyCors")]
    public class ProductCustomerController : ControllerBase
    {
        private readonly IProductCustomerService _productCustomerService;
        public ProductCustomerController(IProductCustomerService productCustomerService)
        {
            _productCustomerService = productCustomerService;
        }
        [HttpGet("GetAllProductCustomers")]
        public async Task<IActionResult> GetAllProductCustomers([FromQuery]string productCode,[FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _productCustomerService.GetAllProductCustomer(productCode,page, pageSize);
                if (!response.Success)
                {
                    return NotFound(new { Success = false, Message = response.Message });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi: " + ex.Message);
            }
        }

        [HttpPost("AddProductCustomer")]
        public async Task<IActionResult> AddProductCustomer([FromBody] ProductCustomerRequestDTO productCustomer)
        {
            if (productCustomer == null)
            {
                return BadRequest("Dữ liệu nhận vào không hợp lệ");
            }
            try
            {
                var response = await _productCustomerService.AddProductCustomer(productCustomer);
                if (!response.Success)
                {
                    return BadRequest(new { Success = false, Message = response.Message });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi: " + ex.Message);
            }
        }

        [HttpPut("UpdateProductCustomer")]
        public async Task<IActionResult> UpdateProductCustomer([FromBody] ProductCustomerRequestDTO productCustomer)
        {
            if (productCustomer == null)
            {
                return BadRequest("Dữ liệu nhận vào không hợp lệ");
            }
            try
            {
                var response = await _productCustomerService.UpdateProductCustomer(productCustomer);
                if (!response.Success)
                {
                    return BadRequest(new { Success = false, Message = response.Message });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi: " + ex.Message);
            }
        }

        [HttpDelete("DeleteProductCustomer")]
        public async Task<IActionResult> DeleteProductCustomer([FromQuery] string productCode, [FromQuery]string customerCode)
        {
            if (string.IsNullOrEmpty(productCode) || string.IsNullOrEmpty(customerCode))
            {
                return BadRequest("Mã khách hàng sản phẩm không hợp lệ");
            }
            try
            {
                var response = await _productCustomerService.DeleteProductCustomer(productCode,customerCode);
                if (!response.Success)
                {
                    return NotFound(new { Success = false, Message = response.Message });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi: " + ex.Message);
            }
        }
    }
}
