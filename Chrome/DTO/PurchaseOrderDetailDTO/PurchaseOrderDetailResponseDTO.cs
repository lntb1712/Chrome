namespace Chrome.DTO.PurchaseOrderDetailDTO
{
    public class PurchaseOrderDetailResponseDTO
    {
        public string PurchaseOrderCode { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public double? Quantity { get; set; }
        public double? QuantityReceived { get; set; }
    }
}
