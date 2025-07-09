namespace Chrome.DTO.DashboardDTO
{
    public class DailyStockInOutDTO
    {
        public string Date { get; set; } = null!;
        public int StockInCount { get; set; }
        public int StockOutCount { get; set; }
    }
}
