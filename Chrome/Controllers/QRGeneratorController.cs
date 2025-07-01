using Chrome.DTO.QRGeneratorDTO;
using Chrome.Services.QRGeneratorService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chrome.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRGeneratorController : ControllerBase
    {
        private readonly IQRGeneratorService _qrGeneratorService;

        public QRGeneratorController(IQRGeneratorService qrGeneratorService)
        {
            _qrGeneratorService = qrGeneratorService;
        }

        [HttpPost("GeneratorQRCode")]
        public async Task<IActionResult> GeneratorQRCode([FromBody] QRGeneratorRequestDTO request)
        {
            try
            {
                var response = await _qrGeneratorService.GenerateAndSaveQRCodeAsync(request);
                if (!response.Success)
                {
                    return NotFound(new
                    {
                        Success = true,
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
    }
}
