namespace Chrome.DTO.BOMComponentDTO
{
    public class BOMComponentRequestDTO
    {
        public string BOMCode { get; set; } = null!;

        public string ComponentCode { get; set; } = null!;

        public string BOMVersion { get; set; } = null!;

        public double? ConsumpQuantity { get; set; }

        public double? ScrapRate { get; set; }
    }
}
