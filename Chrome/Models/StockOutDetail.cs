using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class StockOutDetail
{
    public string StockOutCode { get; set; } = null!;

    public string ProductCode { get; set; } = null!;

    public double? Demand { get; set; }

    public double? Quantity { get; set; }

    public virtual ProductMaster ProductCodeNavigation { get; set; } = null!;

    public virtual StockOut StockOutCodeNavigation { get; set; } = null!;
}
