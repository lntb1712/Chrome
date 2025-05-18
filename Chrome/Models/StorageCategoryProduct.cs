using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class StorageCategoryProduct
{
    public string StorageCategoryId { get; set; } = null!;

    public string ProductCode { get; set; } = null!;

    public double? MaxQuantity { get; set; }

    public virtual ProductMaster ProductCodeNavigation { get; set; } = null!;

    public virtual StorageCategory StorageCategory { get; set; } = null!;
}
