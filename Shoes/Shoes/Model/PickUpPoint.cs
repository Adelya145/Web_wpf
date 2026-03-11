using System;
using System.Collections.Generic;

namespace Shoes.Model;

public partial class PickUpPoint
{
    public int PickUpPointId { get; set; }

    public string? PickUpPointIndex { get; set; }

    public string? PickUpPointCity { get; set; }

    public string? PickUpPointStreet { get; set; }

    public string? PickUpPointHome { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public string PickUpFullAddress => $"{PickUpPointIndex}, {PickUpPointCity}, {PickUpPointStreet}, {PickUpPointHome}";
}
