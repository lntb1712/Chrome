namespace Chrome.DTO.PutAwayDTO
{
    public class PutAwayRequestDTO
    {
        public string PutAwayCode { get; set; } = null!;

        public string? OrderTypeCode { get; set; }

        public string? LocationCode { get; set; }

        public string? Responsible { get; set; }

        public int? StatusId { get; set; }

        public string? PutAwayDate { get; set; }

        public string? PutAwayDescription { get; set; }
    }
}
