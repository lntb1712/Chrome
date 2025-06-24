namespace Chrome.DTO.StockTakeDTO
{
    public class StockTakeRequestDTO
    {
        public string StocktakeCode { get; set; } = null!;

        public string? StocktakeDate { get; set; }

        public string? WarehouseCode { get; set; }

        public string? Responsible { get; set; }

        public int? StatusId { get; set; }

    }
}
