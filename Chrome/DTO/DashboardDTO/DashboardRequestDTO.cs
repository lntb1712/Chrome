namespace Chrome.DTO.DashboardDTO
{
    public class DashboardRequestDTO
    {
        public string[] warehouseCodes { get; set; } = Array.Empty<string>();
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? Quarter { get; set; }
    }
}
