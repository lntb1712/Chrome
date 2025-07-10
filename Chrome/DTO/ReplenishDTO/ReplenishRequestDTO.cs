namespace Chrome.DTO.ReplenishDTO
{
    public class ReplenishRequestDTO
    {
        public string ProductCode { get; set; } = null!;

        public string WarehouseCode { get; set; } = null!;

        public double? MinQuantity { get; set; }

        public double? MaxQuantity { get; set; }
    }
}
