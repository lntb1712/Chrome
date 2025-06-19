using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class Stocktake
{
    public string StocktakeCode { get; set; } = null!;

    public DateTime? StocktakeDate { get; set; }

    public string? WarehouseCode { get; set; }

    public string? Responsible { get; set; }

    public int? StatusId { get; set; }

    public virtual AccountManagement? ResponsibleNavigation { get; set; }

    public virtual StatusMaster? Status { get; set; }

    public virtual WarehouseMaster? WarehouseCodeNavigation { get; set; }
}
