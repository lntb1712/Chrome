using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class GroupManagement
{
    public string GroupId { get; set; } = null!;

    public string? GroupName { get; set; }

    public string? GroupDescription { get; set; }

    public virtual ICollection<AccountManagement> AccountManagements { get; set; } = new List<AccountManagement>();

    public virtual ICollection<GroupFunction> GroupFunctions { get; set; } = new List<GroupFunction>();
}
