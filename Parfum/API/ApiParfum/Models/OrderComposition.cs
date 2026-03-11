using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApiParfum.Models;

public partial class OrderComposition
{
    public int OrderCompositionId { get; set; }

    public int? OrderId { get; set; }

    public string? TovarArticle { get; set; }

    public int? OrderCompositionCount { get; set; }
    [JsonIgnore]
    public virtual Order? Order { get; set; }
    [JsonIgnore]
    public virtual Tovar? TovarArticleNavigation { get; set; }
}
