namespace Chrome.DTO.DashboardDTO
{
    public class UpcomingDeadlineDTO
    {
        public string OrderCode { get; set; } = null!;
        public List<string> ProductCodes { get; set; } = new List<string>();
        public string Deadline { get; set; } = null!;
        public string StatusName { get; set; } = null!;

    }
}
