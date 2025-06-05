using Chrome.DTO.ProductSupplierDTO;
using Chrome.Services.ProductSupplierSerivce;
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
    public class ProductSupplierController : ControllerBase
    {
        private readonly IProductSupplierService _productSupplierService;

        public ProductSupplierController(IProductSupplierService productSupplierService)
        {
            _productSupplierService = productSupplierService;
        }

        [HttpGet("GetAllProductSupplier")]
        public async Task<IActionResult> GetAllProductSupplier([FromRoute] string productCode, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _productSupplierService.GetAllProductSupplier(productCode, page, pageSize);
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi: " + ex.Message);
            }
        }

        [HttpPost("AddProductSupplier")]
        public async Task<IActionResult> AddProductSupplier([FromBody] ProductSupplierRequestDTO productSupplier)
        {
            try
            {
                var response = await _productSupplierService.AddProductSupplier(productSupplier);
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi: " + ex.Message);
            }
        }

        [HttpDelete("DeleteProductSupplier")]
        public async Task<IActionResult> DeleteProductSupplier([FromRoute] string productCode, [FromQuery] string supplierCode)
        {
            try
            {
                var response = await _productSupplierService.DeleteProductSupplier(productCode, supplierCode);
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi: " + ex.Message);
            }
        }

        [HttpPut("UpdateProductSupplier")]
        public async Task<IActionResult> UpdateProductSupplier([FromBody] ProductSupplierRequestDTO productSupplier)
        {
            try
            {
                var response = await _productSupplierService.UpdateProductSupplier(productSupplier);
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi: " + ex.Message);
            }
        }
    }
}
