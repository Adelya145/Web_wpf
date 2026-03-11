using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApiParfum.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateOnly? OrderDate { get; set; }

    public DateOnly? OrderDateDelivery { get; set; }

    public int? PickUpPointId { get; set; }

    public int? UserId { get; set; }

    public string? OrderCode { get; set; }

    public string? OrderStatus { get; set; }
    [JsonIgnore]
    public virtual ICollection<OrderComposition> OrderCompositions { get; set; } = new List<OrderComposition>();
    [JsonIgnore]
    public virtual PickUpPoint? PickUpPoint { get; set; }
    [JsonIgnore]
    public virtual User? User { get; set; }
}
