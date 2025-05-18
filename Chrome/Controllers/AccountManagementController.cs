﻿using Chrome.DTO.AccountManagementDTO;
using Chrome.Services.AccountManagementService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace Chrome.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "PermissionPolicy")]
    [EnableCors("MyCors")]
    public class AccountManagementController : ControllerBase
    {
        private readonly IAccountManagementService _accountManagementService;
        
        public AccountManagementController(IAccountManagementService accountManagementService)
        {
            _accountManagementService = accountManagementService;
        }

        [HttpPost("AddAccountManagement")]
        public async Task<IActionResult> AddAccountManagement([FromBody] AccountManagementRequestDTO accountManagementRequestDTO)
        {
            if (accountManagementRequestDTO == null)
            {
                return BadRequest("Dữ liệu nhận vào không hợp lệ");
            }

            try
            {
                var response = await _accountManagementService.AddAccountManagement(accountManagementRequestDTO);
                if (response.Success == false)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = response.Message
                    });
                }
                return Ok(new { Success = true, Message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi {ex.Message}");
            }
        }

        [HttpGet("GetAllAccount")]
        public async Task<IActionResult> GetAllAccount()
        {
            try
            {
                var response = await _accountManagementService.GetAllAccount();
                if (response == null)
                {
                    return NotFound("Không tìm thấy dữ liệu");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi {ex.Message}");
            }
        }

        [HttpGet("GetAllAccountWithRole")]
        public async Task<IActionResult> GetAllAccountWithRole([FromQuery] string GroupID)
        {
            try
            {
                var response = await _accountManagementService.GetAllAccountWithGroupId(GroupID);
                if (response == null)
                {
                    return NotFound("Không tìm thấy dữ liệu");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi {ex.Message}");
            }
        }

        [HttpDelete("DeleteAccountManagement")]
        public async Task<IActionResult> DeleteAccountManagement([FromQuery] string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                return BadRequest("Dữ liệu nhận vào không hợp lệ");
            }

            try
            {
                var response = await _accountManagementService.DeleteAccountManagement(UserName);
                if (response.Success == false)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = response.Message
                    });
                }
                return Ok(new { Success = true, Message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi {ex.Message}");
            }
        }
        [HttpPut("UpdateAccountManagement")]
        public async Task<IActionResult> UpdateAccountManagement([FromBody] AccountManagementRequestDTO accountManagementRequestDTO)
        {
            if (accountManagementRequestDTO == null)
            {
                return BadRequest("Dữ liệu nhận vào không hợp lệ");
            }
            try
            {
                var response = await _accountManagementService.UpdateAccountManagement(accountManagementRequestDTO);
                if (response.Success == false)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = response.Message
                    });
                }
                return Ok(new { Success = true, Message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi {ex.Message}");
            }
        }
        [HttpGet("SearchAccountInList")]
        public async Task<IActionResult> SearchAccountInList([FromQuery] string textToSearch)
        {
            try
            {
                var response = await _accountManagementService.SearchAccount(textToSearch);
                if (response == null)
                {
                    return NotFound("Không tìm thấy dữ liệu");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi {ex.Message}");
            }

        }

    }
}
