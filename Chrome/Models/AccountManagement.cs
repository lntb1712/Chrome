using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class AccountManagement
{
    public string UserName { get; set; } = null!;

    public string? Password { get; set; }

    public string? FullName { get; set; }

    public string? GroupId { get; set; }

    public virtual GroupManagement? Group { get; set; }

    public virtual ICollection<Movement> Movements { get; set; } = new List<Movement>();

    public virtual ICollection<PickList> PickLists { get; set; } = new List<PickList>();

    public virtual ICollection<PutAway> PutAways { get; set; } = new List<PutAway>();

    public virtual ICollection<StockIn> StockIns { get; set; } = new List<StockIn>();

    public virtual ICollection<StockOut> StockOuts { get; set; } = new List<StockOut>();

    public virtual ICollection<Transfer> Transfers { get; set; } = new List<Transfer>();

    public virtual ICollection<WarehouseMaster> WarehouseMasters { get; set; } = new List<WarehouseMaster>();
}
