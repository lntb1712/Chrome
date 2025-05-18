using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class ProductMaster
{
    public string ProductCode { get; set; } = null!;

    public string? ProductName { get; set; }

    public string? ProductDescription { get; set; }

    public string? ProductImg { get; set; }

    public string? CategoryId { get; set; }

    public double? BaseQuantity { get; set; }

    public string? Uom { get; set; }

    public string? BaseUom { get; set; }

    public DateTime? UpdateTime { get; set; }

    public string? UpdateBy { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<MovementDetail> MovementDetails { get; set; } = new List<MovementDetail>();

    public virtual ICollection<PickListDetail> PickListDetails { get; set; } = new List<PickListDetail>();

    public virtual ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>();

    public virtual ICollection<PutAwayDetail> PutAwayDetails { get; set; } = new List<PutAwayDetail>();

    public virtual ICollection<PutAwayRule> PutAwayRules { get; set; } = new List<PutAwayRule>();

    public virtual ICollection<StockInDetail> StockInDetails { get; set; } = new List<StockInDetail>();

    public virtual ICollection<StockOutDetail> StockOutDetails { get; set; } = new List<StockOutDetail>();

    public virtual ICollection<StockTakeDetail> StockTakeDetails { get; set; } = new List<StockTakeDetail>();

    public virtual ICollection<StorageCategoryProduct> StorageCategoryProducts { get; set; } = new List<StorageCategoryProduct>();

    public virtual ICollection<TransferDetail> TransferDetails { get; set; } = new List<TransferDetail>();
}
