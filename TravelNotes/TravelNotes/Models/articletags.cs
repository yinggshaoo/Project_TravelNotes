using System;
using System.Collections.Generic;

namespace TravelNotes.Models;

public partial class articletags
{
    public int LabelId { get; set; }

    public int ArticleId { get; set; }

    public virtual article Article { get; set; } = null!;

    public virtual Spots Label { get; set; } = null!;
}
