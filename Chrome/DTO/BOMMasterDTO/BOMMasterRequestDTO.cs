namespace Chrome.DTO.BOMMasterDTO
{
    public class BOMMasterRequestDTO
    {
        public string BOMCode { get; set; } = null!;

        public bool? IsActive { get; set; }

        public string BOMVersion { get; set; } = null!;

        public string? ProductCode { get; set; }

    }
}
