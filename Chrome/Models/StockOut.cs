using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class StockOut
{
    public string StockOutCode { get; set; } = null!;

    public string? OrderTypeCode { get; set; }

    public string? WarehouseCode { get; set; }

    public string? CustomerCode { get; set; }

    public string? Responsible { get; set; }

    public int? StatusId { get; set; }

    public DateTime? StockOutDate { get; set; }

    public string? StockOutDescription { get; set; }

    public virtual CustomerMaster? CustomerCodeNavigation { get; set; }

    public virtual OrderType? OrderTypeCodeNavigation { get; set; }

    public virtual AccountManagement? ResponsibleNavigation { get; set; }

    public virtual StatusMaster? Status { get; set; }

    public virtual ICollection<StockOutDetail> StockOutDetails { get; set; } = new List<StockOutDetail>();

    public virtual WarehouseMaster? WarehouseCodeNavigation { get; set; }
}
