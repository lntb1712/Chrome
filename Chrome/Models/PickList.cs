using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class PickList
{
    public string PickNo { get; set; } = null!;

    public string? OrderTypeCode { get; set; }

    public string? LocationCode { get; set; }

    public string? Responsible { get; set; }

    public int? StatusId { get; set; }

    public DateTime? PickDate { get; set; }

    public string? PickDescription { get; set; }

    public virtual LocationMaster? LocationCodeNavigation { get; set; }

    public virtual OrderType? OrderTypeCodeNavigation { get; set; }

    public virtual ICollection<PickListDetail> PickListDetails { get; set; } = new List<PickListDetail>();

    public virtual AccountManagement? ResponsibleNavigation { get; set; }

    public virtual StatusMaster? Status { get; set; }
}
