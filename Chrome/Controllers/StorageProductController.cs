using Chrome.DTO.StorageProductDTO;
using Chrome.Services.StorageProductService;
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
    public class StorageProductController : ControllerBase
    {
        private readonly IStorageProductService _storageProductService;
        public StorageProductController(IStorageProductService storageProductService)
        {
            _storageProductService = storageProductService;
        }

        [HttpGet("GetAllStorageProducts")]
        public async Task<IActionResult> GetAllStorageProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _storageProductService.GetAllStorageProducts(page, pageSize);
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

        [HttpGet("GetStorageProductWithCode")]
        public async Task<IActionResult> GetStorageProductWithCode([FromQuery] string storageProductCode)
        {
            try
            {
                var response = await _storageProductService.GetStorageProductWithCode(storageProductCode);
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

        [HttpGet("GetTotalStorageProductCount")]
        public async Task<IActionResult> GetTotalStorageProductCount()
        {
            try
            {
                var response = await _storageProductService.GetTotalStorageProductCount();
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

        [HttpGet("SearchStorageProducts")]
        public async Task<IActionResult> SearchStorageProducts([FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _storageProductService.SearchStorageProducts(textToSearch, page, pageSize);
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

        [HttpPost("AddStorageProduct")]
        public async Task<IActionResult> AddStorageProduct([FromBody] StorageProductRequestDTO storageProductRequestDTO)
        {
            try
            {
                var response = await _storageProductService.AddStorageProduct(storageProductRequestDTO);
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
        [HttpDelete("DeleteStorageProduct")]
        public async Task<IActionResult> DeleteStorageProduct([FromQuery] string storageProductCode)
        {
            try
            {
                var response = await _storageProductService.DeleteStorageProduct(storageProductCode);
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
        [HttpPut("UpdateStorageProduct")]
        public async Task<IActionResult> UpdateStorageProduct([FromBody] StorageProductRequestDTO storageProductRequestDTO)
        {
            try
            {
                var response = await _storageProductService.UpdateStorageProduct(storageProductRequestDTO);
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
    }
}
