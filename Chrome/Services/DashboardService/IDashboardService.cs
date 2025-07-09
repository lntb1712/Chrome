using Chrome.DTO;
using Chrome.DTO.DashboardDTO;

namespace Chrome.Services.DashboardService
{
    public interface IDashboardService
    {
        Task<ServiceResponse<DashboardResponseDTO>> GetDashboardInformation (DashboardRequestDTO dashboardRequest);
        Task<ServiceResponse<DashboardStockInOutSummaryDTO>> GetStockInOutSummaryAsync(DashboardRequestDTO dashboardRequest);
    }
}
