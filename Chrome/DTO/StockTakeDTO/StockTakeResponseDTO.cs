namespace Chrome.DTO.StockTakeDTO
{
    public class StockTakeResponseDTO
    {
        public string StocktakeCode { get; set; } = null!;

        public string? StocktakeDate { get; set; }

        public string? WarehouseCode { get; set; }
        public string? WarehouseName { get; set; }

        public string? Responsible { get; set; }
        public string? FullNameResponsible { get; set; }

        public int? StatusId { get; set; }
        public string? StatusName { get; set; }

    }
}
