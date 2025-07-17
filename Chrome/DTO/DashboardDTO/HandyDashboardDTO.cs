namespace Chrome.DTO.DashboardDTO
{
    public class HandyDashboardDTO
    {
        public List<string> Alerts { get; set; } = new();
        public List<HandyTaskDTO> TodoTasks { get; set; } = new();
        public Dictionary<string, int> SummaryToday { get; set; } = new();
    }
}
