using Chrome.DTO;
using Chrome.DTO.ManufacturingOrderDetailDTO;
using Chrome.Models;
using Chrome.Repositories.ManufacturingOrderDetailRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Chrome.Services.ManufacturingOrderDetailService
{
    public class ManufacturingOrderDetailService : IManufacturingOrderDetailService
    {
        private readonly IManufacturingOrderDetailRepository _manufacturingOrderDetailRepository;

        public ManufacturingOrderDetailService(IManufacturingOrderDetailRepository manufacturingOrderDetailRepository)
        {
            _manufacturingOrderDetailRepository = manufacturingOrderDetailRepository;
        }

        public async Task<ServiceResponse<PagedResponse<ManufacturingOrderDetailResponseDTO>>> GetManufacturingOrderDetail(string manufacturingOrderCode)
        {
            if (string.IsNullOrEmpty(manufacturingOrderCode))
            {
                return new ServiceResponse<PagedResponse<ManufacturingOrderDetailResponseDTO>>(false, "Mã lệnh sản xuất không được để trống");
            }

            try
            {
                var query = _manufacturingOrderDetailRepository.GetManufacturingOrderDetail(manufacturingOrderCode);
                var result = await query
                    .Select(x => new ManufacturingOrderDetailResponseDTO
                    {
                        ManufacturingOrderCode = x.ManufacturingOrderCode,
                        ComponentCode = x.ComponentCode,
                        ComponentName = x.ComponentCodeNavigation!.ProductName!,
                        ToConsumeQuantity = x.ToConsumeQuantity,
                        ConsumedQuantity = x.ConsumedQuantity,
                        ScraptRate = x.ScraptRate
                    })
                    .OrderBy(x => x.ComponentCode)
                    .ToListAsync();

                var totalItems = await query.CountAsync();
                var pagedResponse = new PagedResponse<ManufacturingOrderDetailResponseDTO>(result, 1, totalItems, totalItems);

                if (!result.Any())
                {
                    return new ServiceResponse<PagedResponse<ManufacturingOrderDetailResponseDTO>>(false, $"Không tìm thấy chi tiết lệnh sản xuất cho mã {manufacturingOrderCode}");
                }

                return new ServiceResponse<PagedResponse<ManufacturingOrderDetailResponseDTO>>(true, "Lấy danh sách chi tiết lệnh sản xuất thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<ManufacturingOrderDetailResponseDTO>>(false, $"Lỗi khi lấy danh sách chi tiết lệnh sản xuất: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<ManufacturingOrderDetailResponseDTO>> GetManufacturingOrderDetail(string manufacturingOrderCode, string productCode)
        {
            if (string.IsNullOrEmpty(manufacturingOrderCode) || string.IsNullOrEmpty(productCode))
            {
                return new ServiceResponse<ManufacturingOrderDetailResponseDTO>(false, "Mã lệnh sản xuất hoặc mã sản phẩm không được để trống");
            }

            try
            {
                var detail = await _manufacturingOrderDetailRepository.GetManufacturingOrderDetailWithCode(manufacturingOrderCode, productCode);
                if (detail == null)
                {
                    return new ServiceResponse<ManufacturingOrderDetailResponseDTO>(false, $"Không tìm thấy chi tiết lệnh sản xuất cho mã {manufacturingOrderCode} và mã sản phẩm {productCode}");
                }

                var response = new ManufacturingOrderDetailResponseDTO
                {
                    ManufacturingOrderCode = detail.ManufacturingOrderCode,
                    ComponentCode = detail.ComponentCode,
                    ComponentName = detail.ComponentCodeNavigation!.ProductName!,
                    ToConsumeQuantity = detail.ToConsumeQuantity,
                    ConsumedQuantity = detail.ConsumedQuantity,
                    ScraptRate = detail.ScraptRate
                };

                return new ServiceResponse<ManufacturingOrderDetailResponseDTO>(true, "Lấy chi tiết lệnh sản xuất thành công", response);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ManufacturingOrderDetailResponseDTO>(false, $"Lỗi khi lấy chi tiết lệnh sản xuất: {ex.Message}");
            }
        }
    }
}