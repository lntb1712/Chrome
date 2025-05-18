using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class PutAwayRule
{
    public string PutAwayRuleCode { get; set; } = null!;

    public string? WarehouseToApply { get; set; }

    public string? ProductCode { get; set; }

    public string? LocationCode { get; set; }

    public string? StorageCategoryId { get; set; }

    public DateTime? UpdateTime { get; set; }

    public string? UpdateBy { get; set; }

    public virtual LocationMaster? LocationCodeNavigation { get; set; }

    public virtual ProductMaster? ProductCodeNavigation { get; set; }

    public virtual StorageCategory? StorageCategory { get; set; }

    public virtual WarehouseMaster? WarehouseToApplyNavigation { get; set; }
}
