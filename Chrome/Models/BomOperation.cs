using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class BomOperation
{
    public string? BomCode { get; set; }

    public string? BomVersion { get; set; }

    public string? OperationCode { get; set; }

    public string? WorkCenterCode { get; set; }

    public TimeOnly? Duration { get; set; }

    public int? Step { get; set; }
}
