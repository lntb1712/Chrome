using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class ReservationDetail
{
    public int Id { get; set; }

    public string? ReservationCode { get; set; }

    public string? ProductCode { get; set; }

    public string? LotNo { get; set; }

    public string? LocationCode { get; set; }

    public double? QuantityReserved { get; set; }

    public virtual Reservation? ReservationCodeNavigation { get; set; }
}
