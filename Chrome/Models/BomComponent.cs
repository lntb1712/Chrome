using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class BomComponent
{
    public string Bomcode { get; set; } = null!;

    public string ComponentCode { get; set; } = null!;

    public string BomVersion { get; set; } = null!;

    public double? ConsumpQuantity { get; set; }

    public double? ScrapRate { get; set; }

    public virtual Bommaster Bommaster { get; set; } = null!;

    public virtual ProductMaster ComponentCodeNavigation { get; set; } = null!;
}
