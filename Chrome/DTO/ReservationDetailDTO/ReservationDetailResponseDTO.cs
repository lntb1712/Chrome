namespace Chrome.DTO.ReservationDetailDTO
{
    public class ReservationDetailResponseDTO
    {
        public string? ReservationCode { get; set; }

        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }

        public string? Lotno { get; set; }

        public string? LocationCode { get; set; }
        public string? LocationName { get; set; }

        public double? QuantityReserved { get; set; }
    }
}
