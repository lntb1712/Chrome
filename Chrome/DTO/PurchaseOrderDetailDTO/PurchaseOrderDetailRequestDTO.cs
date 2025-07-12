namespace Chrome.DTO.PurchaseOrderDetailDTO
{
    public class PurchaseOrderDetailRequestDTO
    {
        public string? PurchaseOrderCode { get; set; } = null!;
        public string? ProductCode { get; set; } = null!;
        public double? Quantity { get; set; } = null!;
        public double? QuantityReceived { get; set; } = null!;

    }
}
