using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class Function
{
    public string FunctionId { get; set; } = null!;

    public string? FunctionName { get; set; }

    public virtual ICollection<GroupFunction> GroupFunctions { get; set; } = new List<GroupFunction>();
}
