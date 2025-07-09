namespace Chrome.DTO.DashboardDTO
{
    public class DashboardStockInOutSummaryDTO
    {
        public int StockInToday { get; set; }
        public int StockInThisWeek { get; set; }
        public int StockInThisMonth { get; set; }

        public int StockOutToday { get; set; }
        public int StockOutThisWeek { get; set; }
        public int StockOutThisMonth { get; set; }

        public List<DailyStockInOutDTO> DailyStockInOuts { get; set; } = new();
    }
}
