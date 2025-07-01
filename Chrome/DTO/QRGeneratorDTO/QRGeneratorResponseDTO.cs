namespace Chrome.DTO.QRGeneratorDTO
{
    public class QRGeneratorResponseDTO
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? FilePath { get; set; }
    }
}
