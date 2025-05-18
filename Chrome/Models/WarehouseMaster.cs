using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class WarehouseMaster
{
    public string WarehouseCode { get; set; } = null!;

    public string? WarehouseName { get; set; }

    public DateTime? UpdateTime { get; set; }

    public string? UpdateBy { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<LocationMaster> LocationMasters { get; set; } = new List<LocationMaster>();

    public virtual ICollection<Movement> Movements { get; set; } = new List<Movement>();

    public virtual ICollection<PutAwayRule> PutAwayRules { get; set; } = new List<PutAwayRule>();

    public virtual ICollection<StockIn> StockIns { get; set; } = new List<StockIn>();

    public virtual ICollection<StockOut> StockOuts { get; set; } = new List<StockOut>();

    public virtual ICollection<StockTake> StockTakes { get; set; } = new List<StockTake>();

    public virtual ICollection<Transfer> TransferFromWarehouseCodeNavigations { get; set; } = new List<Transfer>();

    public virtual ICollection<Transfer> TransferToWarehouseCodeNavigations { get; set; } = new List<Transfer>();
}
