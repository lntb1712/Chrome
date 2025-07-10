namespace Chrome.DTO.ReplenishDTO
{
    public class ReplenishResponseDTO
    {
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;

        public string WarehouseCode { get; set; } = null!;
        public string WarehouseName { get; set; } = null!;

        public double? MinQuantity { get; set; }

        public double? MaxQuantity { get; set; }

        public double? TotalOnHand { get; set; }
    }
}
