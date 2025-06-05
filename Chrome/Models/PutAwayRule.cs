using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class PutAwayRule
{
    public string PutAwaysRuleCode { get; set; } = null!;

    public string? WarehouseToApply { get; set; }

    public string? ProductCode { get; set; }

    public string? LocationCode { get; set; }

    public string? StorageProductId { get; set; }

    public virtual LocationMaster? LocationCodeNavigation { get; set; }

    public virtual ProductMaster? ProductCodeNavigation { get; set; }

    public virtual StorageProduct? StorageProduct { get; set; }

    public virtual WarehouseMaster? WarehouseToApplyNavigation { get; set; }
}
