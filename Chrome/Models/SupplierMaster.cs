using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class SupplierMaster
{
    public string SupplierCode { get; set; } = null!;

    public string? SupplierName { get; set; }

    public string? SupplierPhone { get; set; }

    public string? SupplierAddress { get; set; }

    public virtual ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>();

    public virtual ICollection<StockIn> StockIns { get; set; } = new List<StockIn>();
}
