namespace Chrome.DTO.PutAwayDetailDTO
{
    public class PutAwayDetailRequestDTO
    {
        public string PutAwayCode { get; set; } = null!;

        public string ProductCode { get; set; } = null!;

        public string? LotNo { get; set; }

        public double? Demand { get; set; }

        public double? Quantity { get; set; }

    }
}
