using Chrome.DTO.LoginDTO;
using Chrome.Services.AccountManagementService;
using Chrome.Services.LoginService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace Chrome.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IAccountManagementService _accountManagementService;

        public LoginController(ILoginService loginService, IAccountManagementService accountManagementService)
        {
            _loginService = loginService;
            _accountManagementService = accountManagementService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null)
            {
                return BadRequest("Dữ liệu nhận vào không hợp lệ");
            }
            try
            {
                var response = await _loginService.LoginAsync(request);
                if(!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    return Unauthorized(new
                    {
                        Success = false,
                        Message = response.ErrorMessage
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500,$"Lỗi không xác định: {ex.Message}");
            }
        }

        [HttpGet("GetUserInformation")]
        public async Task<IActionResult> GetUserInformation([FromQuery] string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest("Tên đăng nhập không hợp lệ");
            }
            try
            {
                var response = await _accountManagementService.GetUserInformation(userName);
                if (response == null)
                {
                    return NotFound("Không tìm thấy thông tin người dùng");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi không xác định: {ex.Message}");
            }
        }
    }
}
