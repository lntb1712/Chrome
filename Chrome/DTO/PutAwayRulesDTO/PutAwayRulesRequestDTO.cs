namespace Chrome.DTO.PutAwayRulesDTO
{
    public class PutAwayRulesRequestDTO
    {
        public string PutAwayRuleCode { get; set; } = null!;

        public string? WarehouseToApply { get; set; }

        public string? ProductCode { get; set; }

        public string? LocationCode { get; set; }

        public string? StorageProductId { get; set; }
    }
}
