﻿using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class StockInDetail
{
    public string StockInCode { get; set; } = null!;

    public string ProductCode { get; set; } = null!;

    public string LotNo { get; set; } = null!;

    public double? Demand { get; set; }

    public double? Quantity { get; set; }

    public virtual ProductMaster ProductCodeNavigation { get; set; } = null!;

    public virtual StockIn StockInCodeNavigation { get; set; } = null!;
}
