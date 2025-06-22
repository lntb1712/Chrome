namespace Chrome.DTO.PickListDTO
{
    public class PickListResponseDTO
    {
        public string PickNo { get; set; } = null!;

        public string? ReservationCode { get; set; }

        public string? WarehouseCode { get; set; }
        public string? WarehouseName { get; set; }

        public string? PickDate { get; set; }

        public int? StatusId { get; set; }
        public string? StatusName { get; set; }

    }
}
