namespace Chrome.DTO.InventoryDTO
{
    public class InventoryRequestDTO
    {

        public string LocationCode { get; set; } = null!;

        public string ProductCode { get; set; } = null!;

        public string LotNo { get; set; } = null!;

        public double? Quantity { get; set; }
    }
}
