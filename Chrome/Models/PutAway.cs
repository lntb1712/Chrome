using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class PutAway
{
    public string PutAwayCode { get; set; } = null!;

    public string? OrderTypeCode { get; set; }

    public string? LocationCode { get; set; }

    public string? Responsible { get; set; }

    public int? StatusId { get; set; }

    public DateTime? PutAwayDate { get; set; }

    public string? PutAwayDescription { get; set; }

    public virtual LocationMaster? LocationCodeNavigation { get; set; }

    public virtual OrderType? OrderTypeCodeNavigation { get; set; }

    public virtual ICollection<PutAwayDetail> PutAwayDetails { get; set; } = new List<PutAwayDetail>();

    public virtual AccountManagement? ResponsibleNavigation { get; set; }

    public virtual StatusMaster? Status { get; set; }
}
