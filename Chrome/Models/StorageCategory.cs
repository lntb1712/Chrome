using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class StorageCategory
{
    public string StorageCategoryId { get; set; } = null!;

    public string? StorageCategoryName { get; set; }

    public virtual ICollection<LocationMaster> LocationMasters { get; set; } = new List<LocationMaster>();

    public virtual ICollection<PutAwayRule> PutAwayRules { get; set; } = new List<PutAwayRule>();

    public virtual ICollection<StorageCategoryProduct> StorageCategoryProducts { get; set; } = new List<StorageCategoryProduct>();
}
