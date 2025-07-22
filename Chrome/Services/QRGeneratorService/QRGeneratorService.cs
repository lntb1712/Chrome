using Chrome.DTO;
using Chrome.DTO.QRGeneratorDTO;
using QRCoder;

namespace Chrome.Services.QRGeneratorService
{
    public class QRGeneratorService : IQRGeneratorService
    {
        private string SanitizeFileName(string input)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                input = input.Replace(c, '_');
            }
            return input;
        }

        public async Task<ServiceResponse<QRGeneratorResponseDTO>> GenerateAndSaveQRCodeAsync(QRGeneratorRequestDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.ProductCode)) return new ServiceResponse<QRGeneratorResponseDTO>(false, "Mã sản phẩm không được để trống");
                if (string.IsNullOrEmpty(request.LotNo)) return new ServiceResponse<QRGeneratorResponseDTO>(false, "Số Lot không được để trống");

                string qrData = $"{request.ProductCode}|{request.LotNo}";
                using var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                var pngQrCode = new PngByteQRCode(qrCodeData);
                byte[] imageData = pngQrCode.GetGraphic(20); // 20 pixels per module

                // Base file name and desktop path
                string baseFileName = $"{SanitizeFileName(request.ProductCode)}_{SanitizeFileName(request.LotNo)}";
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fileExtension = ".png";
                // Generate file name with SerialNumber
                string fileName = $"{baseFileName}_{fileExtension}";
                string filePath = Path.Combine(desktopPath, fileName);

                // Check if file exists and append a counter if necessary
                int counter = 1;
                while (File.Exists(filePath))
                {
                    fileName = $"{baseFileName}_{counter}{fileExtension}";
                    filePath = Path.Combine(desktopPath, fileName);
                    counter++;
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await fileStream.WriteAsync(imageData, 0, imageData.Length);
                }

                var qrGeneratorResponse = new QRGeneratorResponseDTO
                {
                    Success = true,
                    FilePath = filePath,
                    Message = $"Mã QR đã được tạo ở {filePath}"
                };

                return new ServiceResponse<QRGeneratorResponseDTO>(true, "Tạo QR thành công",qrGeneratorResponse);
            }catch(Exception ex)
            {
                return new ServiceResponse<QRGeneratorResponseDTO>(false, $"Lỗi: {ex.Message}");
            }

        }
    }
}
