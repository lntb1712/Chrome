namespace Chrome.DTO.LocationMasterDTO
{
    public class LocationMasterResponseDTO
    {
        public string LocationCode { get; set; } = null!;
        public string? LocationName { get; set; }
        public string? WarehouseCode { get; set; }
        public string? WarehouseName { get; set; }
        public string? StorageProductId { get; set; }
        public string? StorageProductName { get; set; }
        public bool? IsEmpty { get; set; }
    }
}
