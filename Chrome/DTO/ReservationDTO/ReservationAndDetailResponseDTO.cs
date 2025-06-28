using Chrome.DTO.ReservationDetailDTO;

namespace Chrome.DTO.ReservationDTO
{
    public class ReservationAndDetailResponseDTO
    {
        public string ReservationCode { get; set; } = null!;

        public string? OrderTypeCode { get; set; }
        public string? OrderTypeName { get; set; }
        public string? WarehouseCode { get; set; }
        public string? WarehouseName { get; set; }

        public string? OrderId { get; set; }

        public string? ReservationDate { get; set; }

        public int? StatusId { get; set; }
        public string? StatusName { get; set; }

        public List<ReservationDetailResponseDTO> reservationDetailResponseDTOs { get; set; } = new List<ReservationDetailResponseDTO>();
    }
}
