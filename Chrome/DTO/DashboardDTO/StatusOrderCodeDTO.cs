namespace Chrome.DTO.DashboardDTO
{
    public class StatusOrderCodeDTO
    {
        public string OrderTypeCode { get; set; } = null!;
        public int CountStatusStart { get; set; }
        public int CountStatusInProgress { get; set; }
        public int CountStatusCompleted { get; set; }
    }
}
