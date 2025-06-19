using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class Bommaster
{
    public string Bomcode { get; set; } = null!;

    public bool? IsActive { get; set; }

    public string Bomversion { get; set; } = null!;

    public string? ProductCode { get; set; }

    public virtual ICollection<BomComponent> BomComponents { get; set; } = new List<BomComponent>();

    public virtual ICollection<ManufacturingOrder> ManufacturingOrders { get; set; } = new List<ManufacturingOrder>();

    public virtual ProductMaster? ProductCodeNavigation { get; set; }
}
