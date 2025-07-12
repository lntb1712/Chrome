using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class StockIn
{
    public string StockInCode { get; set; } = null!;

    public string? OrderTypeCode { get; set; }

    public string? WarehouseCode { get; set; }

    public string? PurchaseOrderCode { get; set; }

    public string? Responsible { get; set; }

    public int? StatusId { get; set; }

    public DateTime? OrderDeadline { get; set; }

    public string? StockInDescription { get; set; }

    public virtual OrderType? OrderTypeCodeNavigation { get; set; }

    public virtual PurchaseOrder? PurchaseOrderCodeNavigation { get; set; }

    public virtual AccountManagement? ResponsibleNavigation { get; set; }

    public virtual StatusMaster? Status { get; set; }

    public virtual ICollection<StockInDetail> StockInDetails { get; set; } = new List<StockInDetail>();

    public virtual WarehouseMaster? WarehouseCodeNavigation { get; set; }
}
