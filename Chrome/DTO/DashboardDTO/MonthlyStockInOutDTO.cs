namespace Chrome.DTO.DashboardDTO
{
    public class MonthlyStockInOutDTO
    {
        public string Month { get; set; }
        public int StockInCount { get; set; }
        public int StockOutCount { get; set; }
        public int PurchaseOrderCount { get; set; }
        public int StocktakeCount { get; set; }
    }
}
