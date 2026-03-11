using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApiParfum.Models;

public partial class Tovar
{
    public string TovarArticle { get; set; } = null!;

    public string? TovarName { get; set; }

    public string? TovarUnit { get; set; }

    public decimal? TovarCost { get; set; }

    public int? SupplierId { get; set; }

    public int? ManufactrurerId { get; set; }

    public int? TovarCategoryId { get; set; }

    public int? TovarCurrentSale { get; set; }

    public int? TovarCount { get; set; }

    public string? TovarDesc { get; set; }

    public string? TovarPhoto { get; set; }

    public virtual ICollection<Bascet> Bascets { get; set; } = new List<Bascet>();
    [JsonIgnore]
    public virtual Manufacturer? Manufactrurer { get; set; }
    [JsonIgnore]
    public virtual ICollection<OrderComposition> OrderCompositions { get; set; } = new List<OrderComposition>();
    [JsonIgnore]
    public virtual Supplier? Supplier { get; set; }
    [JsonIgnore]
    public virtual TovarCategory? TovarCategory { get; set; }
    public string? SupplierName => Supplier?.SupplierName;

    public string? ManufacturerName => Manufactrurer?.ManufacturerName;

    public string? TovarCategoryName => TovarCategory?.TovarCategoryName;
}
