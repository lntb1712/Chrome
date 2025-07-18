using Chrome.DTO.StockTakeDTO;
using Chrome.Services.StockTakeService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "PermissionPolicy")]
[EnableCors("MyCors")]
public class StockTakeController : ControllerBase
{
    private readonly IStockTakeService _StockTakeService;

    public StockTakeController(IStockTakeService StockTakeService)
    {
        _StockTakeService = StockTakeService ?? throw new ArgumentNullException(nameof(StockTakeService));
    }

    [HttpGet("GetAllStockTakes")]
    public async Task<IActionResult> GetAllStockTakes([FromQuery] string[] warehouseCodes, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var response = await _StockTakeService.GetAllStockTakesAsync(warehouseCodes, page, pageSize);
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
    [HttpGet("GetAllStockTakesAsyncWithResponsible")]
    public async Task<IActionResult> GetAllStockTakesAsyncWithResponsible([FromQuery] string[] warehouseCodes,[FromQuery]string responsible, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var response = await _StockTakeService.GetAllStockTakesAsyncWithResponsible(warehouseCodes,responsible, page, pageSize);
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

    [HttpGet("GetStockTakesByStatus")]
    public async Task<IActionResult> GetStockTakesByStatus([FromQuery] string[] warehouseCodes, [FromQuery] int statusId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var response = await _StockTakeService.GetStockTakesByStatusAsync(warehouseCodes, statusId, page, pageSize);
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

    [HttpGet("SearchStockTakes")]
    public async Task<IActionResult> SearchStockTakes([FromQuery] string[] warehouseCodes, [FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var response = await _StockTakeService.SearchStockTakesAsync(warehouseCodes, textToSearch, page, pageSize);
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
    [HttpGet("SearchStockTakesAsyncWithResponsible")]
    public async Task<IActionResult> SearchStockTakesAsyncWithResponsible([FromQuery] string[] warehouseCodes,[FromQuery]string responsible ,[FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var response = await _StockTakeService.SearchStockTakesAsyncWithResponsible(warehouseCodes,responsible, textToSearch, page, pageSize);
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

    [HttpPost("AddStockTake")]
    public async Task<IActionResult> AddStockTake([FromBody] StockTakeRequestDTO StockTake)
    {
        try
        {
            var response = await _StockTakeService.AddStockTake(StockTake);
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

    [HttpPut("UpdateStockTake")]
    public async Task<IActionResult> UpdateStockTake([FromBody] StockTakeRequestDTO StockTake)
    {
        try
        {
            var response = await _StockTakeService.UpdateStockTake(StockTake);
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
    [HttpPut("ConfirmnStockTake")]
    public async Task<IActionResult> ConfirmnStockTake([FromBody] StockTakeRequestDTO StockTake)
    {
        try
        {
            var response = await _StockTakeService.ConfirmnStockTake(StockTake);
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

    [HttpDelete("DeleteStockTake")]
    public async Task<IActionResult> DeleteStockTake([FromQuery] string StockTakeCode)
    {
        try
        {
            string decodedStockTake = Uri.UnescapeDataString(StockTakeCode);
            var response = await _StockTakeService.DeleteStockTakeAsync(decodedStockTake);
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

    [HttpGet("GetListResponsible")]
    public async Task<IActionResult> GetListResponsible([FromQuery] string warehouseCode)
    {
        try
        {
            var response = await _StockTakeService.GetListResponsibleAsync(warehouseCode);
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

    [HttpGet("GetListStatusMaster")]
    public async Task<IActionResult> GetListStatusMaster()
    {
        try
        {
            var response = await _StockTakeService.GetListStatusMaster();
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

    [HttpGet("GetListWarehousePermission")]
    public async Task<IActionResult> GetListWarehousePermission([FromQuery] string[] warehouseCodes)
    {
        try
        {
            var response = await _StockTakeService.GetListWarehousePermission(warehouseCodes);
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
}