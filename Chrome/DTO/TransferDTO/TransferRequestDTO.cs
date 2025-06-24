namespace Chrome.DTO.TransferDTO
{
    public class TransferRequestDTO
    {
        public string TransferCode { get; set; } = null!;

        public string? OrderTypeCode { get; set; }

        public string? FromWarehouseCode { get; set; }

        public string? ToWarehouseCode { get; set; }

        public string? ToResponsible { get; set; }

        public string? FromResponsible { get; set; }

        public int? StatusId { get; set; }

        public string? TransferDate { get; set; }

        public string? TransferDescription { get; set; }
    }
}
