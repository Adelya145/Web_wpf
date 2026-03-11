using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace Shoes.Model;

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

    public virtual Manufacturer? Manufactrurer { get; set; }

    public virtual ICollection<OrderComposition> OrderCompositions { get; set; } = new List<OrderComposition>();

    public virtual Supplier? Supplier { get; set; }

    public virtual TovarCategory? TovarCategory { get; set; }
    public string Name_Category => $"{TovarCategory.TovarCategoryName} | {TovarName}";
    public string DisplayedImage
    {
        get
        {
            string im = "C:\\Users\\79174\\Downloads\\Telegram Desktop\\Материал к заданию\\Материалы к заданию\\Shoes\\Shoes\\Resources\\";
            
            string imagePath = im + TovarPhoto;
            if (File.Exists(imagePath))
            {
                return imagePath;
            }
            else
            {
                return im + "picture.png";
            }
            
        }
    }

    public string DiscountFon => TovarCurrentSale > 15 ? "#2E8B57" : "transparent";
    public bool HasDiscount => TovarCurrentSale.HasValue && TovarCurrentSale.Value > 0;
    public decimal? FinalPrice
    {
        get
        {
            if (!HasDiscount || !TovarCost.HasValue)
                return TovarCost;

            return TovarCost.Value * (100 - TovarCurrentSale.Value) / 100;
        }
    }

    public string DisplayPrice
    {
        get
        {
            if (!HasDiscount || !TovarCost.HasValue)
                return $"{TovarCost} ₽";

            return $"{TovarCost} ₽ → {FinalPrice} ₽";
        }
    }
    public bool IsOutOfStock => !TovarCount.HasValue || TovarCount.Value <= 0;
    public string ItemBackgroundColor
    {
        get
        {
            if (IsOutOfStock)
                return "Blue";
            return "Black";
        }
    }
    [NotMapped]
    public string DisplayName { get; set; }
}
