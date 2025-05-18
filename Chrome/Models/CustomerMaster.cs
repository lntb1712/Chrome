using System;
using System.Collections.Generic;

namespace Chrome.Models;

public partial class CustomerMaster
{
    public string CustomerCode { get; set; } = null!;

    public string? CustomerName { get; set; }

    public string? CustomerPhone { get; set; }

    public string? CustomerAddress { get; set; }

    public string? Email { get; set; }

    public DateTime? UpdateTime { get; set; }

    public string? UpdateBy { get; set; }

    public virtual ICollection<StockOut> StockOuts { get; set; } = new List<StockOut>();
}
