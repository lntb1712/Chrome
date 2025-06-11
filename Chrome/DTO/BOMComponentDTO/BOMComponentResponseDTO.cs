namespace Chrome.DTO.BOMComponentDTO
{
    public class BOMComponentResponseDTO
    {
        public string BOMCode { get; set; } = null!;
        public string ComponentCode { get; set; } = null!;

        public string ComponentName { get; set; } = null!;

        public string BOMVersion { get; set; } = null!;

        public double? ConsumpQuantity { get; set; }

        public double? ScrapRate { get; set; }
    }
}
