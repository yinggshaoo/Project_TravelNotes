using System;
using System.Collections.Generic;

namespace TravelNoteDevelop.Models;

public partial class articletags
{
    public int LabelId { get; set; }

    public int? ArticleId { get; set; }

    public string LabelName { get; set; } = null!;

    public string? LabelDescription { get; set; }

    public virtual article? Article { get; set; }
}
