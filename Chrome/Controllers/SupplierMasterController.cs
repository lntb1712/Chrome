using Chrome.DTO.ProductSupplierDTO;
using Chrome.DTO.SupplierMasterDTO;
using Chrome.Services.ProductSupplierSerivce;
using Chrome.Services.SupplierMasterService;
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
    public class SupplierMasterController : ControllerBase
    {
        private readonly ISupplierMasterService _supplierMasterService;
       
        public SupplierMasterController(ISupplierMasterService supplierMasterService)
        {
            _supplierMasterService = supplierMasterService;
        }
        //Supplier Master
        [HttpGet("GetAllSupplierMaster")]
        public async Task<IActionResult> GetAllSupplierMaster([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _supplierMasterService.GetAllSupplierMaster(page, pageSize);
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi" + ex.Message);
            }
        }

        [HttpGet("GetSupplierWithSupplierCode")]
        public async Task<IActionResult> GetSupplierWithSupplierCode([FromQuery] string supplierCode)
        {
            try
            {
                var response = await _supplierMasterService.GetSupplierWithSupplierCode(supplierCode);
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi:" + ex.Message);
            }
        }

        [HttpGet("SearchSupplier")]
        public async Task<IActionResult> SearchSupplier([FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _supplierMasterService.SearchSupplier(textToSearch, page, pageSize);
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

        [HttpGet("GetTotalSupplierCount")]
        public async Task<IActionResult> GetTotalSupplierCount()
        {
            try
            {
                var response = await _supplierMasterService.GetTotalSupplierCount();
                if (!response.Success)
                {
                    return BadRequest(new
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

        [HttpPost("AddSupplierMaster")]
        public async Task<IActionResult> AddSupplierMaster([FromBody] SupplierMasterRequestDTO supplierMaster)
        {
            try
            {
                var response = await _supplierMasterService.AddSupplierMaster(supplierMaster);
                if(!response.Success)
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

        [HttpDelete("DeleteSupplierMaster")]
        public async Task<IActionResult> DeleteSupplierMaster([FromQuery] string supplierCode)
        {
            try
            {
                var response = await _supplierMasterService.DeleteSupplierMaster(supplierCode);
                if(!response.Success)
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

        [HttpPut("UpdateSupplierMaster")]
        public async Task<IActionResult> UpdateSupplierMaster([FromBody] SupplierMasterRequestDTO supplierMaster)
        {
            try
            {
                var response = await _supplierMasterService.UpdateSupplierMaster(supplierMaster);
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

