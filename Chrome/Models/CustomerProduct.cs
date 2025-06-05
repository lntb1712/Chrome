using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class CustomerProduct
{
    public string CustomerCode { get; set; } = null!;

    public string ProductCode { get; set; } = null!;

    public int? ExpectedDeliverTime { get; set; }

    public double? PricePerUnit { get; set; }

    public virtual CustomerMaster CustomerCodeNavigation { get; set; } = null!;

    public virtual ProductMaster ProductCodeNavigation { get; set; } = null!;
}
