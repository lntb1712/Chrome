using Chrome.DTO;
using Chrome.DTO.ReservationDetailDTO;
using Chrome.Models;
using Chrome.Repositories.ReservationDetailRepository;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Services.ReservationDetailService
{
    public class ReservationDetailService : IReservationDetailService
    {
        private readonly IReservationDetailRepository _reservationDetailRepository;
        private readonly ChromeContext _context;

        public ReservationDetailService(IReservationDetailRepository reservationDetailRepository, ChromeContext context)
        {
            _reservationDetailRepository = reservationDetailRepository ?? throw new ArgumentNullException(nameof(reservationDetailRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ServiceResponse<PagedResponse<ReservationDetailResponseDTO>>> GetAllReservationDetails(string reservationCode, int page, int pageSize)
        {
            
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                string decodeReservationCode = Uri.UnescapeDataString(reservationCode);
                var query = _reservationDetailRepository.GetAllReservationDetailsAsync(decodeReservationCode);
                var totalItems = await query.CountAsync();
                var details = await query
                    .Select(x => new ReservationDetailResponseDTO
                    {
                        ReservationCode = x.ReservationCode,
                        ProductCode = x.ProductCode,
                        ProductName = x.ProductCodeNavigation!.ProductName,
                        Lotno = x.Lotno,
                        LocationCode = x.LocationCode,
                        LocationName = x.LocationCodeNavigation!.LocationName,
                        QuantityReserved = x.QuantityReserved
                    })
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var pagedResponse = new PagedResponse<ReservationDetailResponseDTO>(details, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<ReservationDetailResponseDTO>>(true, "Lấy danh sách chi tiết reservation thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<ReservationDetailResponseDTO>>(false, $"Lỗi khi lấy danh sách chi tiết reservation: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<ReservationDetailResponseDTO>> GetReservationDetailByIdAsync(int id)
        {
            try
            {
                var detail = await _reservationDetailRepository.GetReservationDetailByIdAsync(id);
                if (detail == null)
                    return new ServiceResponse<ReservationDetailResponseDTO>(false, "Chi tiết reservation không tồn tại");

                var response = new ReservationDetailResponseDTO
                {
                    ReservationCode = detail.ReservationCode,
                    ProductCode = detail.ProductCode,
                    ProductName = detail.ProductCodeNavigation!.ProductName,
                    Lotno = detail.Lotno,
                    LocationCode = detail.LocationCode,
                    LocationName = detail.LocationCodeNavigation!.LocationName,
                    QuantityReserved = detail.QuantityReserved
                };
                return new ServiceResponse<ReservationDetailResponseDTO>(true, "Lấy chi tiết reservation thành công", response);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ReservationDetailResponseDTO>(false, $"Lỗi khi lấy chi tiết reservation: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<ReservationDetailResponseDTO>>> SearchReservationDetailsAsync(string reservationCode, string textToSearch, int page, int pageSize)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                string decodeReservationCode = Uri.UnescapeDataString(reservationCode);
                var query = _reservationDetailRepository.SearchReservationDetailsAsync(decodeReservationCode, textToSearch);
                var totalItems = await query.CountAsync();
                var details = await query
                    .Select(x => new ReservationDetailResponseDTO
                    {
                        ReservationCode = x.ReservationCode,
                        ProductCode = x.ProductCode,
                        ProductName = x.ProductCodeNavigation!.ProductName,
                        Lotno = x.Lotno,
                        LocationCode = x.LocationCode,
                        LocationName = x.LocationCodeNavigation!.LocationName,
                        QuantityReserved = x.QuantityReserved
                    })
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var pagedResponse = new PagedResponse<ReservationDetailResponseDTO>(details, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<ReservationDetailResponseDTO>>(true, "Tìm kiếm chi tiết reservation thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<ReservationDetailResponseDTO>>(false, $"Lỗi khi tìm kiếm chi tiết reservation: {ex.Message}");
            }
        }
    }
}
