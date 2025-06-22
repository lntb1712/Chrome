namespace Chrome.DTO.ReservationDetailDTO
{
    public class ReservationDetailRequestDTO
    {

        public string? ReservationCode { get; set; }

        public string? ProductCode { get; set; }

        public string? Lotno { get; set; }

        public string? LocationCode { get; set; }

        public double? QuantityReserved { get; set; }

    }
}
