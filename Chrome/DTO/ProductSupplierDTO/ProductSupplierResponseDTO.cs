namespace Chrome.DTO.ProductSupplierDTO
{
    public class ProductSupplierResponseDTO
    {
        public string SupplierCode { get; set; } = null!;
        public string SupplierName { get; set; } = null!;

        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;

        public int? LeadTime { get; set; }

        public double? PricePerUnit { get; set; }
    }
}
