using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class ManufWorkOrder
{
    public string? ManufacturingOrderCode { get; set; }

    public string? OperationCode { get; set; }

    public string? WorkCenterCode { get; set; }

    public TimeOnly? ExpectedDuration { get; set; }

    public TimeOnly? RealDuration { get; set; }
}
