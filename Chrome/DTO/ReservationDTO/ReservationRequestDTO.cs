namespace Chrome.DTO.ReservationDTO
{
    public class ReservationRequestDTO
    {
        public string ReservationCode { get; set; } = null!;
        public string? WarehouseCode { get; set; }

        public string? OrderTypeCode { get; set; }

        public string? OrderId { get; set; }

        public string? ReservationDate { get; set; }

        public int? StatusId { get; set; }

    }
}
