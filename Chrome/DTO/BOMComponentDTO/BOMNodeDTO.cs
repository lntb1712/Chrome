namespace Chrome.DTO.BOMComponentDTO
{
    public class BOMNodeDTO
    {
        public string BOMCode { get; set; } =null!;
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string BOMVersion { get; set; } = null!;
        public string ComponentCode { get; set; } = null!;
        public string ComponentName { get; set; } = null!;
        public float TotalQuantity { get; set; }
        public int Level { get; set; }
    }
}
