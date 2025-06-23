namespace Chrome.DTO.MovementDTO
{
    public class MovementRequestDTO
    {
        public string MovementCode { get; set; } = null!;

        public string? OrderTypeCode { get; set; }

        public string? WarehouseCode { get; set; }

        public string? FromLocation { get; set; }

        public string? ToLocation { get; set; }

        public string? Responsible { get; set; }

        public int? StatusId { get; set; }

        public string? MovementDate { get; set; }

        public string? MovementDescription { get; set; }
    }
}
