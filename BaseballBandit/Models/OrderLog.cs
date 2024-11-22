using System;
using System.Collections.Generic;

namespace BaseballBandit.Models;

public partial class OrderLog
{
    public int OrderNum { get; set; }

    public int UserId { get; set; }

    public string ShippingAddress { get; set; } = null!;

    public double Total { get; set; }

    public DateTime OrderDate { get; set; }

    public bool Refunded { get; set; }

    public int PaymentID { get; set; }
}
