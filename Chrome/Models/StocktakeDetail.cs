using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class StocktakeDetail
{
    public string StocktakeCode { get; set; } = null!;

    public string? ProductCode { get; set; }

    public string? Lotno { get; set; }

    public string? LocationCode { get; set; }

    public double? Quantity { get; set; }

    public double? CountedQuantity { get; set; }

    public virtual LocationMaster? LocationCodeNavigation { get; set; }

    public virtual ProductMaster? ProductCodeNavigation { get; set; }
}
