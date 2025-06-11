namespace Chrome.DTO.BOMMasterDTO
{
    public class BOMMasterResponseDTO
    {
        public string BOMCode { get; set; } = null!;
        public List<BOMVersionResponseDTO> BOMVersionResponses { get; set; } = new List<BOMVersionResponseDTO>();

        public string? ProductCode { get; set; }

        public string? ProductName { get; set; }
    }
}
