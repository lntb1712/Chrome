namespace Chrome.DTO.StockOutDTO
{
    public class StockOutRequestDTO
    {
        public string StockOutCode { get; set; } = null!;

        public string? OrderTypeCode { get; set; }

        public string? WarehouseCode { get; set; }

        public string? CustomerCode { get; set; }

        public string? Responsible { get; set; }

        public int? StatusId { get; set; }

        public string? StockOutDate { get; set; }

        public string? StockOutDescription { get; set; }
    }
}
