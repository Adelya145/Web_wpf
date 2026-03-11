using System;
using System.Collections.Generic;

namespace Shoes.Model;

public partial class OrderComposition
{
    public int OrderCompositionId { get; set; }

    public int? OrderId { get; set; }

    public string? TovarArticle { get; set; }

    public int? OrderCompositionCount { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Tovar? TovarArticleNavigation { get; set; }
}
