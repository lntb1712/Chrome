using Chrome.DTO;
using Chrome.DTO.OrderTypeDTO;
using Chrome.DTO.ReservationDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.DTO.WarehouseMasterDTO;

namespace Chrome.Services.ReservationService
{
    public interface IReservationService
    {
        Task<ServiceResponse<PagedResponse<ReservationResponseDTO>>> GetAllReservations(string[] warehouseCodes, int page, int pageSize);
        Task<ServiceResponse<PagedResponse<ReservationResponseDTO>>> GetAllReservationsWithStatus(string[] warehouseCodes, int statusId, int page, int pageSize);
        Task<ServiceResponse<PagedResponse<ReservationResponseDTO>>> SearchReservationsAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize);
        Task<ServiceResponse<bool>> AddReservation(ReservationRequestDTO reservation);
        Task<ServiceResponse<bool>> DeleteReservationAsync(string reservationCode);
        Task<ServiceResponse<bool>> UpdateReservation(ReservationRequestDTO reservation);
        Task<ServiceResponse<List<OrderTypeResponseDTO>>> GetListOrderType(string prefix);
        Task<ServiceResponse<List<StatusMasterResponseDTO>>> GetListStatusMaster();
        Task<ServiceResponse<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission(string[] warehouseCodes);
        Task<ServiceResponse<ReservationResponseDTO>> GetReservationsByStockOutCodeAsync(string stockOutCode);
        Task<ServiceResponse<ReservationResponseDTO>> GetReservationsByMovementCodeAsync(string movementCode);
        Task<ServiceResponse<ReservationResponseDTO>> GetReservationsByTransferCodeAsync(string transferCode);
    }
}
