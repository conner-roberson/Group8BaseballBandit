using System;
using System.Collections.Generic;

namespace BaseballBandit.Models;

public partial class Inventory
{
    public int ProductId { get; set; }

    public double ProductPrice { get; set; }

    public int NumInStock { get; set; }

    public string ProductType { get; set; } = null!;

    public string ProductColor { get; set; } = null!;

    public int? ProductEquipmentSize { get; set; }

    public string? ProductApparelSize { get; set; }

    public string? ImagePath { get; set; }

    public string? Name { get; set; }

    public string? Brand { get; set; }
}
