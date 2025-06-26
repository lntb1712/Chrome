using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class ManufacturingOrder
{
    public string ManufacturingOrderCode { get; set; } = null!;

    public string? OrderTypeCode { get; set; }

    public string? ProductCode { get; set; }

    public string? Bomcode { get; set; }

    public string? BomVersion { get; set; }

    public int? Quantity { get; set; }

    public int? QuantityProduced { get; set; }

    public DateTime? ScheduleDate { get; set; }

    public DateTime? Deadline { get; set; }

    public string? Responsible { get; set; }

    public string? Lotno { get; set; }

    public int? StatusId { get; set; }

    public string? WarehouseCode { get; set; }

    public virtual Bommaster? Bommaster { get; set; }

    public virtual ICollection<ManufacturingOrderDetail> ManufacturingOrderDetails { get; set; } = new List<ManufacturingOrderDetail>();

    public virtual OrderType? OrderTypeCodeNavigation { get; set; }

    public virtual ProductMaster? ProductCodeNavigation { get; set; }

    public virtual AccountManagement? ResponsibleNavigation { get; set; }

    public virtual StatusMaster? Status { get; set; }

    public virtual WarehouseMaster? WarehouseCodeNavigation { get; set; }
}
