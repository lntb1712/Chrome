using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class StockTake
{
    public string StockTakeCode { get; set; } = null!;

    public string? WarehouseCode { get; set; }

    public string? Responsible { get; set; }

    public int? StatusId { get; set; }

    public DateTime? TransferDate { get; set; }

    public string? TransferDescription { get; set; }

    public virtual AccountManagement? ResponsibleNavigation { get; set; }

    public virtual StatusMaster? Status { get; set; }

    public virtual ICollection<StockTakeDetail> StockTakeDetails { get; set; } = new List<StockTakeDetail>();

    public virtual WarehouseMaster? WarehouseCodeNavigation { get; set; }
}
