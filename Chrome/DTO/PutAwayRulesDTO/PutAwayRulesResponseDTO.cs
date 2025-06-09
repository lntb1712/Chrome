namespace Chrome.DTO.PutAwayRulesDTO
{
    public class PutAwayRulesResponseDTO
    {
        public string PutAwayRuleCode { get; set; } = null!;
        public string? WarehouseToApply { get; set; }
        public string? WarehouseToApplyName { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string? LocationCode { get; set; }
        public string? LocationName { get; set; }
        public string? StorageProductId { get; set; }
        public string? StorageProductName { get; set; }
    }
}
