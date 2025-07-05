namespace Chrome.DTO.ManufacturingOrderDetailDTO
{
    public class ForecastManufacturingOrderDetailDTO
    {
        public string ManufacturingOrderCode { get; set; } = null!;

        public string ComponentCode { get; set; } = null!;
        public double? QuantityOnHand { get; set; }

        public double? QuantityToOutBound { get; set; }

        public double? QuantityToInBound { get; set; }

        public double? AvailableQty { get; set; }
    }
}
