using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class Replenish
{
    public string ProductCode { get; set; } = null!;

    public string WarehouseCode { get; set; } = null!;

    public double? MinQuantity { get; set; }

    public double? MaxQuantity { get; set; }

    public virtual ProductMaster ProductCodeNavigation { get; set; } = null!;

    public virtual WarehouseMaster WarehouseCodeNavigation { get; set; } = null!;
}
