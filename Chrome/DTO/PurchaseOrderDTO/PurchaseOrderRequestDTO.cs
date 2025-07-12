namespace Chrome.DTO.PurchaseOrderDTO
{
    public class PurchaseOrderRequestDTO
    {
        public string PurchaseOrderCode { get; set; } = null!;

        public string? WarehouseCode { get; set; }

        public int? StatusId { get; set; }

        public string? OrderDate { get; set; }

        public string? ExpectedDate { get; set; }

        public string? SupplierCode { get; set; }

        public string? PurchaseOrderDescription { get; set; }
    }
}
