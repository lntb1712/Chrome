namespace Chrome.DTO.StockOutDetailDTO
{
    public class StockOutDetailReportDTO
    {
        public string StockOutCode { get; set; } = null!;

        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string UOM { get; set; } = null!;

        public double? Demand { get; set; }

        public double? Quantity { get; set; }
    }
}
