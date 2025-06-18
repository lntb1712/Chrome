namespace Chrome.DTO.StockOutDTO
{
    public class StockOutResponseDTO
    {
        public string StockOutCode { get; set; } = null!;

        public string? OrderTypeCode { get; set; }
        public string? OrderTypeName { get; set; }

        public string? WarehouseCode { get; set; }
        public string? WarehouseName { get; set; }

        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }

        public string? Responsible { get; set; }
        public string? FullNameResponsible { get; set; }

        public int? StatusId { get; set; }
        public string? StatusName { get; set; }

        public string? StockOutDate { get; set; }

        public string? StockOutDescription { get; set; }
    }
}
