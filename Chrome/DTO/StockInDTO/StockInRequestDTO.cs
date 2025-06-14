namespace Chrome.DTO.StockInDTO
{
    public class StockInRequestDTO
    {
        public string StockInCode { get; set; } = null!;

        public string? OrderTypeCode { get; set; }

        public string? WarehouseCode { get; set; }

        public string? SupplierCode { get; set; }

        public string? Responsible { get; set; }
        
        public int StatusId { get; set; }

        public string? OrderDeadline { get; set; }

        public string? StockInDescription { get; set; }
    }
}
