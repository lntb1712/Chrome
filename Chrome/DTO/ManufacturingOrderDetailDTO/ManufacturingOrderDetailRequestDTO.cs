namespace Chrome.DTO.ManufacturingOrderDetailDTO
{
    public class ManufacturingOrderDetailRequestDTO
    {
        public string ManufacturingOrderCode { get; set; } = null!;

        public string ComponentCode { get; set; } = null!;

        public double? ToConsumeQuantity { get; set; }

        public double? ConsumedQuantity { get; set; }

        public double? ScraptRate { get; set; }
    }
}
