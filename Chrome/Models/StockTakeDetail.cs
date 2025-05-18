using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class StockTakeDetail
{
    public string StockTakeCode { get; set; } = null!;

    public string LocationCode { get; set; } = null!;

    public string ProductCode { get; set; } = null!;

    public string LotNo { get; set; } = null!;

    public double? Demand { get; set; }

    public double? Quantity { get; set; }

    public virtual LocationMaster LocationCodeNavigation { get; set; } = null!;

    public virtual ProductMaster ProductCodeNavigation { get; set; } = null!;

    public virtual StockTake StockTakeCodeNavigation { get; set; } = null!;
}
