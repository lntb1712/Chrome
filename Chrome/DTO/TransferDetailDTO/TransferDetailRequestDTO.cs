namespace Chrome.DTO.TransferDetailDTO
{
    public class TransferDetailRequestDTO
    {
        public string TransferCode { get; set; } = null!;

        public string ProductCode { get; set; } = null!;

        public double? Demand { get; set; }

        public double? QuantityInBounded { get; set; }

        public double? QuantityOutBounded { get; set; }
    }
}
