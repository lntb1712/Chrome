using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class StocktakeDetail
{
    public string StocktakeCode { get; set; } = null!;

    public string ProductCode { get; set; } = null!;

    public string Lotno { get; set; } = null!;

    public string LocationCode { get; set; } = null!;

    public double? Quantity { get; set; }

    public double? CountedQuantity { get; set; }

    public virtual LocationMaster LocationCodeNavigation { get; set; } = null!;

    public virtual ProductMaster ProductCodeNavigation { get; set; } = null!;

    public virtual Stocktake StocktakeCodeNavigation { get; set; } = null!;
}
