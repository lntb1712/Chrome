using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class Movement
{
    public string MovementCode { get; set; } = null!;

    public string? OrderTypeCode { get; set; }

    public string? WarehouseCode { get; set; }

    public string? FromLocation { get; set; }

    public string? ToLocation { get; set; }

    public string? Responsible { get; set; }

    public int? StatusId { get; set; }

    public DateTime? TransferDate { get; set; }

    public string? TransferDescription { get; set; }

    public virtual LocationMaster? FromLocationNavigation { get; set; }

    public virtual OrderType? OrderTypeCodeNavigation { get; set; }

    public virtual AccountManagement? ResponsibleNavigation { get; set; }

    public virtual StatusMaster? Status { get; set; }

    public virtual LocationMaster? ToLocationNavigation { get; set; }

    public virtual WarehouseMaster? WarehouseCodeNavigation { get; set; }
}
