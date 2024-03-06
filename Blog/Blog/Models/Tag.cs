using System;
using System.Collections.Generic;

namespace Blog.Models;

public partial class Tag
{
    public int LabelId { get; set; }

    public int? ArticleId { get; set; }

    public string LabelName { get; set; } = null!;

    public string? LabelDescription { get; set; }

    public virtual Article? Article { get; set; }
}
