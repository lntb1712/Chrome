using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class TransferDetail
{
    public string TransferCode { get; set; } = null!;

    public string ProductCode { get; set; } = null!;

    public double? Demand { get; set; }

    public double? QuantityInBounded { get; set; }

    public double? QuantityOutBounded { get; set; }

    public virtual ProductMaster ProductCodeNavigation { get; set; } = null!;

    public virtual Transfer TransferCodeNavigation { get; set; } = null!;
}
