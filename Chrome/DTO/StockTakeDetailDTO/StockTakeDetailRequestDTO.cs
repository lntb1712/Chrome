namespace Chrome.DTO.StockTakeDetailDTO
{
    public class StockTakeDetailRequestDTO
    {
        public string StocktakeCode { get; set; } = null!;

        public string ProductCode { get; set; } = null!;

        public string Lotno { get; set; } = null!;

        public string LocationCode { get; set; } = null!;

        public double? Quantity { get; set; }

        public double? CountedQuantity { get; set; }
    }
}
