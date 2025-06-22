using Chrome.DTO;
using Chrome.DTO.ReservationDetailDTO;

namespace Chrome.Services.ReservationDetailService
{
    public interface IReservationDetailService
    {
        Task<ServiceResponse<PagedResponse<ReservationDetailResponseDTO>>> GetAllReservationDetails(string reservationCode, int page, int pageSize);
        Task<ServiceResponse<ReservationDetailResponseDTO>> GetReservationDetailByIdAsync(int id);
        Task<ServiceResponse<PagedResponse<ReservationDetailResponseDTO>>> SearchReservationDetailsAsync(string reservationCode, string textToSearch, int page, int pageSize);
    }
}
