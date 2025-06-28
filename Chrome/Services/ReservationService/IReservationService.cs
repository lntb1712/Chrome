using Chrome.DTO;
using Chrome.DTO.OrderTypeDTO;
using Chrome.DTO.ReservationDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.DTO.WarehouseMasterDTO;
using Microsoft.EntityFrameworkCore.Storage;

namespace Chrome.Services.ReservationService
{
    public interface IReservationService
    {
        Task<ServiceResponse<PagedResponse<ReservationResponseDTO>>> GetAllReservations(string[] warehouseCodes, int page, int pageSize);
        Task<ServiceResponse<PagedResponse<ReservationResponseDTO>>> GetAllReservationsWithStatus(string[] warehouseCodes, int statusId, int page, int pageSize);
        Task<ServiceResponse<PagedResponse<ReservationResponseDTO>>> SearchReservationsAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize);
        Task<ServiceResponse<bool>> AddOrUpdateReservation(ReservationRequestDTO reservation,IDbContextTransaction transaction = null!);
        Task<ServiceResponse<bool>> DeleteReservationAsync(string reservationCode);
        Task<ServiceResponse<List<OrderTypeResponseDTO>>> GetListOrderType(string prefix);
        Task<ServiceResponse<List<StatusMasterResponseDTO>>> GetListStatusMaster();
        Task<ServiceResponse<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission(string[] warehouseCodes);
        Task<ServiceResponse<ReservationAndDetailResponseDTO>> GetReservationsByStockOutCodeAsync(string stockOutCode);
        Task<ServiceResponse<ReservationAndDetailResponseDTO>> GetReservationsByMovementCodeAsync(string movementCode);
        Task<ServiceResponse<ReservationAndDetailResponseDTO>> GetReservationsByTransferCodeAsync(string transferCode);
        Task<ServiceResponse<ReservationAndDetailResponseDTO>> GetReservationsByManufacturingCodeAsync(string manufacturingCode);
    }
}
