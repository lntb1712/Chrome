namespace Chrome.DTO.ProductMasterDTO
{
    public class ProductShortageDTO
    {
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public double RequiredQuantity { get; set; }
        public double ShortageQuantity { get; set; }
        public string? WarehouseCode { get; set; }
    }
}
