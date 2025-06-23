using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class MovementDetail
{
    public string MovementCode { get; set; } = null!;

    public string ProductCode { get; set; } = null!;

    public double? Demand { get; set; }

    public virtual Movement MovementCodeNavigation { get; set; } = null!;

    public virtual ProductMaster ProductCodeNavigation { get; set; } = null!;
}
