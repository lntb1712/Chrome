using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class TransferDetail
{
    public string? TransferCode { get; set; }

    public string? ProductCode { get; set; }

    public double? Demand { get; set; }

    public double? Quantity { get; set; }
}
