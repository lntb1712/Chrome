using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class Transfer
{
    public string TransferCode { get; set; } = null!;

    public string? OrderTypeCode { get; set; }

    public string? FromWarehouseCode { get; set; }

    public string? ToWarehouseCode { get; set; }

    public string? Responsible { get; set; }

    public int? StatusId { get; set; }

    public DateTime? TransferDate { get; set; }

    public string? TransferDescription { get; set; }

    public virtual WarehouseMaster? FromWarehouseCodeNavigation { get; set; }

    public virtual OrderType? OrderTypeCodeNavigation { get; set; }

    public virtual AccountManagement? ResponsibleNavigation { get; set; }

    public virtual StatusMaster? Status { get; set; }

    public virtual WarehouseMaster? ToWarehouseCodeNavigation { get; set; }

    public virtual ICollection<TransferDetail> TransferDetails { get; set; } = new List<TransferDetail>();
}
