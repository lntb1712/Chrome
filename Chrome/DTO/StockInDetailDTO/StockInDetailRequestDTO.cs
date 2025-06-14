namespace Chrome.DTO.StockInDetailDTO
{
    public class StockInDetailRequestDTO
    {
        public string StockInCode { get; set; } = null!;

        public string ProductCode { get; set; } = null!;

        public double? Demand { get; set; }

        public double? Quantity { get; set; }
    }
}
