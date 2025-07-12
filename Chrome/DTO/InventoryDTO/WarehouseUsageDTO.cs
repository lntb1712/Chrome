namespace Chrome.DTO.InventoryDTO
{
    public class WarehouseUsageDTO
    {
        public string? WarehouseCode { get; set; }
        public string? WarehouseName { get; set; }
        public List<LocationUsageDTO> locationUsageDTOs { get; set; } = new List<LocationUsageDTO>();
    }
}
