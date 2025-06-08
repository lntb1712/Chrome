using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class LocationMaster
{
    public string LocationCode { get; set; } = null!;

    public string? LocationName { get; set; }

    public string? WarehouseCode { get; set; }

    public string? StorageProductId { get; set; }

    public bool? IsEmpty { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<Movement> MovementFromLocationNavigations { get; set; } = new List<Movement>();

    public virtual ICollection<Movement> MovementToLocationNavigations { get; set; } = new List<Movement>();

    public virtual ICollection<PickListDetail> PickListDetails { get; set; } = new List<PickListDetail>();

    public virtual ICollection<PutAwayRule> PutAwayRules { get; set; } = new List<PutAwayRule>();

    public virtual ICollection<PutAway> PutAways { get; set; } = new List<PutAway>();

    public virtual ICollection<ReservationDetail> ReservationDetails { get; set; } = new List<ReservationDetail>();

    public virtual StorageProduct? StorageProduct { get; set; }

    public virtual WarehouseMaster? WarehouseCodeNavigation { get; set; }
}
