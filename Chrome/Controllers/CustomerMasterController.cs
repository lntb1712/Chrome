using Chrome.DTO.CustomerMasterDTO;
using Chrome.Services.CustomerMasterService;
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
    public class CustomerMasterController : ControllerBase
    {
        private readonly ICustomerMasterService _customerMasterService;
        public CustomerMasterController(ICustomerMasterService customerMasterService)
        {
            _customerMasterService = customerMasterService;
        }

        [HttpGet("GetAllCustomerMaster")]
        public async Task<IActionResult> GetAllCustomerMaster([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _customerMasterService.GetAllCustomerMaster(page, pageSize);
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

        [HttpGet("GetCustomerWithCustomerCode")]
        public async Task<IActionResult> GetCustomerWithCustomerCode([FromQuery] string customerCode)
        {
            try
            {
                var response = await _customerMasterService.GetCustomerWithCustomerCode(customerCode);
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

        [HttpPost("AddCustomerMaster")]
        public async Task<IActionResult> AddCustomerMaster([FromBody] CustomerMasterRequestDTO customer)
        {
            if (customer == null)
            {
                return BadRequest("Dữ liệu nhận vào không hợp lệ");
            }

            try
            {
                var response = await _customerMasterService.AddCustomerMaster(customer);
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

        [HttpPut("UpdateCustomerMaster")]
        public async Task<IActionResult> UpdateCustomerMaster([FromBody] CustomerMasterRequestDTO customer)
        {
            if (customer == null)
            {
                return BadRequest("Dữ liệu nhận vào không hợp lệ");
            }

            try
            {
                var response = await _customerMasterService.UpdateCustomerMaster(customer);
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

        [HttpDelete("DeleteCustomerMaster")]
        public async Task<IActionResult> DeleteCustomerMaster([FromQuery] string customerCode)
        {
            if (string.IsNullOrEmpty(customerCode))
            {
                return BadRequest("Mã khách hàng không hợp lệ");
            }
            try
            {
                var response = await _customerMasterService.DeleteCustomerMaster(customerCode);
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

        [HttpGet("SearchCustomer")]
        public async Task<IActionResult> SearchCustomer([FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _customerMasterService.SearchCustomer(textToSearch, page, pageSize);
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

        [HttpGet("GetTotalCustomerCount")]
        public async Task<IActionResult> GetTotalCustomerCount()
        {
            try
            {
                var response = await _customerMasterService.GetTotalCustomerCount();
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
