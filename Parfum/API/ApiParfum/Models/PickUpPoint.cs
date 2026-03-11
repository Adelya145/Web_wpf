using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApiParfum.Models;

public partial class PickUpPoint
{
    public int PickUpPointId { get; set; }

    public string? PickUpPointIndex { get; set; }

    public string? PickUpPointCity { get; set; }

    public string? PickUpPointStreet { get; set; }

    public string? PickUpPointHome { get; set; }
    [JsonIgnore]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
