namespace Chrome.DTO.LocationMasterDTO
{
    public class LocationMasterRequestDTO
    {
        public string LocationCode { get; set; } = null!;

        public string? LocationName { get; set; }

        public string? WarehouseCode { get; set; }

        public string? StorageProductId { get; set; }
    }
}
