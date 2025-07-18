using Chrome.Services.CodeGeneratorService;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chrome.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CodeGeneratorController : ControllerBase
    {
        private readonly ICodeGeneratorService _codeGeneratorService;
        public CodeGeneratorController(ICodeGeneratorService codeGeneratorService)
        {
            _codeGeneratorService = codeGeneratorService;
        }
        [HttpGet("GenerateCodeAsync")]
        public async Task<IActionResult> GenerateCodeAsync([FromQuery]string warehouseCode,[FromQuery]   string type)
        {
            try
            {
                var response = await _codeGeneratorService.GenerateCodeAsync(warehouseCode,type);
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
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi : {ex.Message}");
            }
        }
    }
}
