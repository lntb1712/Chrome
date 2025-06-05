namespace Chrome.DTO.ProductMasterDTO
{
    public class ProductMasterResponseDTO
    {
        public string ProductCode { get; set; } = null!;
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductImage { get; set; }
        public string? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public double? BaseQuantity { get; set; }
        public string? Uom { get; set; }
        public string? BaseUom { get; set; }
        public float? Valuation { get; set; }
        public float? TotalOnHand { get; set; }
    }
}
