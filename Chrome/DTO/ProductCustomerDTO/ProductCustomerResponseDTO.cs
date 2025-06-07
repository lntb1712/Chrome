namespace Chrome.DTO.ProductCustomerDTO
{
    public class ProductCustomerResponseDTO
    {
        public string CustomerCode { get; set; } = null!;

        public string ProductCode { get; set; } = null!;

        public int? ExpectedDeliverTime { get; set; }

        public double? PricePerUnit { get; set; }
    }
}
