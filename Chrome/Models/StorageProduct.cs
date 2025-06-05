using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class StorageProduct
{
    public string StorageProductId { get; set; } = null!;

    public string? StorageProductName { get; set; }

    public string? ProductCode { get; set; }

    public double? MaxQuantity { get; set; }

    public virtual ICollection<LocationMaster> LocationMasters { get; set; } = new List<LocationMaster>();

    public virtual ProductMaster? ProductCodeNavigation { get; set; }

    public virtual ICollection<PutAwayRule> PutAwayRules { get; set; } = new List<PutAwayRule>();
}
