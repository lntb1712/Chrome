using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class WarehouseAccount
{
    public string WarehouseCode { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public DateTime? AssignedDate { get; set; }

    public virtual AccountManagement UserNameNavigation { get; set; } = null!;

    public virtual WarehouseMaster WarehouseCodeNavigation { get; set; } = null!;
}
