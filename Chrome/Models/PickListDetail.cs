using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class PickListDetail
{
    public int Id { get; set; }

    public string? PickNo { get; set; }

    public string? ProductCode { get; set; }

    public string? LotNo { get; set; }

    public double? Demand { get; set; }

    public double? Quantity { get; set; }

    public string? LocationCode { get; set; }

    public virtual PickList? PickNoNavigation { get; set; }
}
