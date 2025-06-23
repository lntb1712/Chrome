namespace Chrome.DTO.MovementDetailDTO
{
    public class MovementDetailResponseDTO
    {
        public string MovementCode { get; set; } = null!;

        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;

        public double? Demand { get; set; }
    }
}
