using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class Reservation
{
    public string ReservationCode { get; set; } = null!;

    public string? OrderTypeCode { get; set; }

    public string? OrderId { get; set; }

    public DateTime? ReservationDate { get; set; }

    public int? StatusId { get; set; }

    public virtual OrderType? OrderTypeCodeNavigation { get; set; }

    public virtual ICollection<PickList> PickLists { get; set; } = new List<PickList>();

    public virtual ICollection<ReservationDetail> ReservationDetails { get; set; } = new List<ReservationDetail>();

    public virtual StatusMaster? Status { get; set; }
}
