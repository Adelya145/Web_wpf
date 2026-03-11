using System;
using System.Collections.Generic;

namespace Shoes.Model;

public partial class Order
{
    public int OrderId { get; set; }

    public DateOnly? OrderDate { get; set; }

    public DateOnly? OrderDateDelivery { get; set; }

    public int? PickUpPointId { get; set; }

    public int? UserId { get; set; }

    public string? OrderCode { get; set; }

    public string? OrderStatus { get; set; }

    public virtual ICollection<OrderComposition> OrderCompositions { get; set; } = new List<OrderComposition>();

    public virtual PickUpPoint? PickUpPoint { get; set; }

    public virtual User? User { get; set; }
    public string PickUpFullAddress => $"{PickUpPoint.PickUpPointIndex}, {PickUpPoint.PickUpPointCity}, {PickUpPoint.PickUpPointStreet}, {PickUpPoint.PickUpPointHome}";
    public string SNL => $"{User.UserSurname} {User.UserName} {User.UserLastname}";
}
