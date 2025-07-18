using Chrome.DTO;
using Chrome.Models;
using Chrome.Repositories.ManufacturingOrderRepository;
using Chrome.Repositories.MovementRepository;
using Chrome.Repositories.PurchaseOrderRepository;
using Chrome.Repositories.StockInDetailRepository;
using Chrome.Repositories.StockInRepository;
using Chrome.Repositories.StockOutRepository;
using Chrome.Repositories.StockTakeRepository;
using Chrome.Repositories.TransferRepository;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Services.CodeGeneratorService
{
    public class CodeGeneratorService : ICodeGeneratorService 
    {
        private readonly ChromeContext _context;
        public CodeGeneratorService(ChromeContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse< string>> GenerateCodeAsync(string warehouseCode,string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return new ServiceResponse<string>(false, "Loại lệnh không được để trống");
            }
            string upperType = type.ToUpper(); // Đảm bảo viết hoa: "po" -> "PO"
            string datePart = DateTime.Today.ToString("yyMMdd");
            string codePrefix = upperType switch
            {
                // Không cần warehouseCode cho TF, STK
                "TF"=> $"{upperType}{datePart}",
                _ when !string.IsNullOrWhiteSpace(warehouseCode) => $"{warehouseCode}/{upperType}{datePart}",
                _ => throw new ArgumentException("Thiếu mã kho cho loại lệnh cần warehouseCode")
            };

            int count = upperType switch
            {
                "MO" => await _context.ManufacturingOrders
                    .CountAsync(x => x.ManufacturingOrderCode.StartsWith(codePrefix)),

                "PO" => await _context.PurchaseOrders
                    .CountAsync(x => x.PurchaseOrderCode.StartsWith(codePrefix)),

                "SI" => await _context.StockIns
                    .CountAsync(x => x.StockInCode.StartsWith(codePrefix)),

                "SO" => await _context.StockOuts
                    .CountAsync(x => x.StockOutCode.StartsWith(codePrefix)),
                "MV" => await _context.Movements
                .CountAsync(x => x.MovementCode.StartsWith(codePrefix)),
                "TF" => await _context.Transfers
                    .CountAsync(x => x.TransferCode.StartsWith(codePrefix)),
                "STK" => await _context.Stocktakes
                .CountAsync(x => x.StocktakeCode.StartsWith(codePrefix)),

                _ => throw new ArgumentException($"Unknown order type: {type}")
            };

            return new ServiceResponse<string>(true,"Tạo mã thành công", $"{codePrefix}{(count + 1):D3}");
        }

    }
}
