namespace Chrome.DTO.CategoryDTO
{
    public class CategorySummaryDTO
    {
        public string CategoryId { get; set; } = null!;
        public string? CategoryName { get; set; }
        public int TotalProducts { get; set; }
    }
}
