namespace Chrome.DTO.StockOutDetailDTO
{
    public class ForecastStockOutDetailDTO
    {
        public string StockOutCode { get; set; } = null!;

        public string ProductCode { get; set; } = null!;

        public double? QuantityOnHand { get; set; }

        public double? QuantityToOutBound { get; set; }

        public double? QuantityToInBound { get; set; }

        public double? AvailableQty { get; set; }
    }
}
