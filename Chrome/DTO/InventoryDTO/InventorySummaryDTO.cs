namespace Chrome.DTO.InventoryDTO
{
    public class InventorySummaryDTO
    {
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string CategoryId { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public double? BaseQuantity { get; set; } = null!;
        public string UOM { get; set; } = null!;
        public string BaseUOM { get; set; } = null!;
        public double? Quantity { get; set; }
    }
}
