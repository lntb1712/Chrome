namespace Chrome.DTO.MovementDetailDTO
{
    public class MovementDetailRequestDTO
    {
        public string MovementCode { get; set; } = null!;

        public string ProductCode { get; set; } = null!;

        public double? Demand { get; set; }

    }
}
