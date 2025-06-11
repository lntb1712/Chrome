namespace Chrome.DTO.InventoryDTO
{
    public class LocationDetailDTO
    {
        public string WarehouseCode { get; set; } = null!;
        public string WarehouseName { get; set; } = null!;
        public string LocationCode { get; set; } = null!;
        public string LocationName { get; set; } = null!;
        public double? BaseQuantity { get; set; } = null!;
        public string UOM { get; set; } = null!;
        public string BaseUOM { get; set; } = null!;
        public double? Quantity { get; set; }
    }
}
