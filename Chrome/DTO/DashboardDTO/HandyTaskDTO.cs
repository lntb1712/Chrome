namespace Chrome.DTO.DashboardDTO
{
    public class HandyTaskDTO
    {
        public string OrderCode { get; set; } = string.Empty;
        public string OrderType { get; set; } = string.Empty;
        public string Deadline { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<string> ProductCodes { get; set; } = new();
    }
}
