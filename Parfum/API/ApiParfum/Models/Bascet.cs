using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApiParfum.Models;

public partial class Bascet
{
    public int UserId { get; set; }

    public string TovarArticle { get; set; } = null!;

    public int? BascetCount { get; set; }

    public virtual Tovar TovarArticleNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
