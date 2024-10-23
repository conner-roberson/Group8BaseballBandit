using System;
using System.Collections.Generic;

namespace BaseballBandit.Models;

public partial class Cart
{
    public int FkUserId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

}
