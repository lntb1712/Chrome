using Chrome.DTO;
using Chrome.DTO.QRGeneratorDTO;

namespace Chrome.Services.QRGeneratorService
{
    public interface IQRGeneratorService
    {
        Task<ServiceResponse<QRGeneratorResponseDTO>> GenerateAndSaveQRCodeAsync(QRGeneratorRequestDTO request);
    }
}
