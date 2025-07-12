using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class PurchaseOrderDetail
{
    public string PurchaseOrderCode { get; set; } = null!;

    public string ProductCode { get; set; } = null!;

    public double? Quantity { get; set; }

    public double? QuantityReceived { get; set; }

    public virtual ProductMaster ProductCodeNavigation { get; set; } = null!;

    public virtual PurchaseOrder PurchaseOrderCodeNavigation { get; set; } = null!;
}
