using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class OrderType
{
    public string OrderTypeCode { get; set; } = null!;

    public string? OrderTypeName { get; set; }

    public virtual ICollection<ManufacturingOrder> ManufacturingOrders { get; set; } = new List<ManufacturingOrder>();

    public virtual ICollection<Movement> Movements { get; set; } = new List<Movement>();

    public virtual ICollection<PutAway> PutAways { get; set; } = new List<PutAway>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<StockIn> StockIns { get; set; } = new List<StockIn>();

    public virtual ICollection<StockOut> StockOuts { get; set; } = new List<StockOut>();

    public virtual ICollection<Transfer> Transfers { get; set; } = new List<Transfer>();
}
