namespace Chrome.DTO.StockOutDetailDTO
{
    public class StockOutDetailRequestDTO
    {
        public string StockOutCode { get; set; } = null!;

        public string ProductCode { get; set; } = null!;

        public double? Demand { get; set; }

        public double? Quantity { get; set; }
    }
}
