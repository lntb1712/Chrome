namespace Chrome.DTO.PickListDetailDTO
{
    public class PickListDetailRequestDTO
    {
        public string PickNo { get; set; } = null!;

        public string ProductCode { get; set; } = null!;

        public string? LotNo { get; set; }

        public double? Demand { get; set; }

        public double? Quantity { get; set; }

        public string? LocationCode { get; set; }

    }
}
