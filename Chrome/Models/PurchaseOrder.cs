using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class PurchaseOrder
{
    public string PurchaseOrderCode { get; set; } = null!;

    public string? WarehouseCode { get; set; }

    public int? StatusId { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? ExpectedDate { get; set; }

    public string? SupplierCode { get; set; }

    public string? PurchaseOrderDescription { get; set; }

    public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();

    public virtual StatusMaster? Status { get; set; }

    public virtual ICollection<StockIn> StockIns { get; set; } = new List<StockIn>();

    public virtual SupplierMaster? SupplierCodeNavigation { get; set; }

    public virtual WarehouseMaster? WarehouseCodeNavigation { get; set; }
}
