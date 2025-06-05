namespace Chrome.DTO.ProductSupplierDTO
{
    public class ProductSupplierRequestDTO
    {
        public string SupplierCode { get; set; } = null!;

        public string ProductCode { get; set; } = null!;

        public int? LeadTime { get; set; }

        public double? PricePerUnit { get; set; }
    }
}
