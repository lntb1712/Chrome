using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class Category
{
    public string CategoryId { get; set; } = null!;

    public string? CategoryName { get; set; }

    public virtual ICollection<ProductMaster> ProductMasters { get; set; } = new List<ProductMaster>();
}
