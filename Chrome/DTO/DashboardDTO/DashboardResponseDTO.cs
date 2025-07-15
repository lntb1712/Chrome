using Chrome.DTO.InventoryDTO;
using Chrome.DTO.ManufacturingOrderDTO;

namespace Chrome.DTO.DashboardDTO
{
    public class DashboardResponseDTO
    {
        public List<InventorySummaryDTO> InventorySummaryDTOs { get; set; } = new List<InventorySummaryDTO>();
        public List<ProgressManufacturingOrderDTO> ProgressManufacturingOrderDTOs { get; set; } = new List<ProgressManufacturingOrderDTO>();
        public int QuantityToCompleteStockIn { get; set; }
        public int QuantityToCompleteStockOut { get; set; }
        public int QuantityToCompleteManufacturingOrder { get; set; }
        public int QuantityToCompletePurchaseOrder { get; set; }
        public int QuantityToCompleteStocktake { get; set; }
        public List<UpcomingDeadlineDTO> UpcomingDeadlines { get; set; } = new List<UpcomingDeadlineDTO>();
        public List<StatusOrderCodeDTO> StatusOrderCodeDTOs { get; set; } = new List<StatusOrderCodeDTO>();
    }
}
