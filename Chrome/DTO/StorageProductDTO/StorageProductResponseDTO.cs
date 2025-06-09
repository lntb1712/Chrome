namespace Chrome.DTO.StorageProductDTO
{
    public class StorageProductResponseDTO
    {
        public string StorageProductId { get; set; } = null!;

        public string? StorageProductName { get; set; }

        public string? ProductCode { get; set; }

        public string? ProductName { get; set; }

        public double? MaxQuantity { get; set; }
    }
}
