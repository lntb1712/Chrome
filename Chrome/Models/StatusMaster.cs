﻿using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class StatusMaster
{
    public int StatusId { get; set; }

    public string? StatusName { get; set; }

    public virtual ICollection<Movement> Movements { get; set; } = new List<Movement>();

    public virtual ICollection<PickList> PickLists { get; set; } = new List<PickList>();

    public virtual ICollection<PutAway> PutAways { get; set; } = new List<PutAway>();

    public virtual ICollection<StockIn> StockIns { get; set; } = new List<StockIn>();

    public virtual ICollection<StockOut> StockOuts { get; set; } = new List<StockOut>();

    public virtual ICollection<StockTake> StockTakes { get; set; } = new List<StockTake>();

    public virtual ICollection<Transfer> Transfers { get; set; } = new List<Transfer>();
}
