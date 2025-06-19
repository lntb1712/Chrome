using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class WorkCenterMaster
{
    public string WorkCenterCode { get; set; } = null!;

    public string? WorkCenterName { get; set; }

    public string WarehouseCode { get; set; } = null!;

    public virtual ICollection<ManufacturingOrder> ManufacturingOrders { get; set; } = new List<ManufacturingOrder>();

    public virtual WarehouseMaster WarehouseCodeNavigation { get; set; } = null!;
}
