namespace Chrome.DTO.WarehouseMasterDTO
{
    public class WarehouseMasterRequestDTO
    {
        public string WarehouseCode { get; set; } = null!;

        public string? WarehouseName { get; set; }

        public string? WarehouseDescription { get; set; }

        public string? WarehouseAddress { get; set; }

    }
}
