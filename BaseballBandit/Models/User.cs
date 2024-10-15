using System;
using System.Collections.Generic;

namespace BaseballBandit.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string HashedPass { get; set; } = null!;

    public bool Admin { get; set; }

    public bool Seller { get; set; }

    public string Email { get; set; } = null!;

    public string? Address { get; set; }

    public string? AddressCity { get; set; }

    public string? AddressState { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public int AddressZip { get; set; }


}
