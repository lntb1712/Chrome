using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class ManufactoringOrderDetail
{
    public string ManufacturingOrderCode { get; set; } = null!;

    public string ComponentCode { get; set; } = null!;

    public double? ToConsumeQuantity { get; set; }

    public double? ConsumedQuantity { get; set; }

    public double? ScraptRate { get; set; }

    public virtual ProductMaster ComponentCodeNavigation { get; set; } = null!;
}
