using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class PutAwayDetail
{
    public string PutAwayCode { get; set; } = null!;

    public string ProductCode { get; set; } = null!;

    public string? LotNo { get; set; }

    public double? Demand { get; set; }

    public double? Quantity { get; set; }

    public virtual ProductMaster ProductCodeNavigation { get; set; } = null!;

    public virtual PutAway PutAwayCodeNavigation { get; set; } = null!;
}
