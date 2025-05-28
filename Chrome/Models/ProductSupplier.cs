using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class ProductSupplier
{
    public string SupplierCode { get; set; } = null!;

    public string ProductCode { get; set; } = null!;

    public int? LeadTime { get; set; }

    public DateTime? UpdateTime { get; set; }

    public string? UpdateBy { get; set; }

    public double? Quantity { get; set; }

    public virtual ProductMaster ProductCodeNavigation { get; set; } = null!;

    public virtual SupplierMaster SupplierCodeNavigation { get; set; } = null!;
}
