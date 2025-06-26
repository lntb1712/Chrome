using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class PickListDetail
{
    public string PickNo { get; set; } = null!;

    public string ProductCode { get; set; } = null!;

    public string LotNo { get; set; } = null!;

    public double? Demand { get; set; }

    public double? Quantity { get; set; }

    public string? LocationCode { get; set; }

    public virtual LocationMaster? LocationCodeNavigation { get; set; }

    public virtual PickList PickNoNavigation { get; set; } = null!;

    public virtual ProductMaster ProductCodeNavigation { get; set; } = null!;
}
