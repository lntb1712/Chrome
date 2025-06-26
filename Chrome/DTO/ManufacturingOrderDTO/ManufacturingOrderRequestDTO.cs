namespace Chrome.DTO.ManufacturingOrderDTO
{
    public class ManufacturingOrderRequestDTO
    {
        public string ManufacturingOrderCode { get; set; } = null!;

        public string? OrderTypeCode { get; set; }

        public string ProductCode { get; set; } = null!;

        public string Bomcode { get; set; } = null!;
        public string BomVersion { get; set; } = null!;

        public int? Quantity { get; set; }

        public int? QuantityProduced { get; set; }

        public string? ScheduleDate { get; set; }

        public string? Deadline { get; set; }

        public string? Responsible { get; set; }

        public int? StatusId { get; set; }

        public string? WarehouseCode { get; set; }
    }
}
