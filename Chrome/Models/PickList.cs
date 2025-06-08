using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class PickList
{
    public string PickNo { get; set; } = null!;

    public string? ReservationCode { get; set; }

    public string? WarehouseCode { get; set; }

    public DateTime? PickDate { get; set; }

    public int? StatusId { get; set; }

    public virtual ICollection<PickListDetail> PickListDetails { get; set; } = new List<PickListDetail>();

    public virtual Reservation? ReservationCodeNavigation { get; set; }

    public virtual StatusMaster? Status { get; set; }

    public virtual WarehouseMaster? WarehouseCodeNavigation { get; set; }
}
