namespace Chrome.DTO.DashboardDTO
{
    public class HandyDashboardRequestDTO
    {
        public string[] warehouseCodes { get; set; } = Array.Empty<string>();
        public string? userName { get; set; } = string.Empty;
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? Quarter { get; set; }
    }
}
