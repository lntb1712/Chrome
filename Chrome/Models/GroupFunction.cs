using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class GroupFunction
{
    public string GroupId { get; set; } = null!;

    public string FunctionId { get; set; } = null!;

    public bool? IsEnable { get; set; }

    public string ApplicableLocation { get; set; } = null!;

    public virtual WarehouseMaster ApplicableLocationNavigation { get; set; } = null!;

    public virtual Function Function { get; set; } = null!;

    public virtual GroupManagement Group { get; set; } = null!;
}
