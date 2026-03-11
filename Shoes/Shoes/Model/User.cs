using System;
using System.Collections.Generic;

namespace Shoes.Model;

public partial class User
{
    public int UserId { get; set; }

    public string? UserRole { get; set; }

    public string? UserSurname { get; set; }

    public string? UserName { get; set; }

    public string? UserLastname { get; set; }

    public string? UserLogin { get; set; }

    public string? UserPassword { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public string SNL => $"{UserSurname} {UserName} {UserLastname}";
}
