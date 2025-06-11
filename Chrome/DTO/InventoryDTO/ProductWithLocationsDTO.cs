namespace Chrome.DTO.InventoryDTO
{
    public class ProductWithLocationsDTO
    {
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string CategoryId { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public List<LocationDetailDTO> Locations { get; set; } = new();
    }
}
