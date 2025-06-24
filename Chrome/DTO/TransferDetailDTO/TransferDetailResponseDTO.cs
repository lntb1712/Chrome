namespace Chrome.DTO.TransferDetailDTO
{
    public class TransferDetailResponseDTO
    {
        public string TransferCode { get; set; } = null!;

        public string ProductCode { get; set; } = null!;

        public string ProductName { get; set; } = null!;

        public double? Demand { get; set; }

        public double? QuantityInBounded { get; set; }

        public double? QuantityOutBounded { get; set; }
    }
}
