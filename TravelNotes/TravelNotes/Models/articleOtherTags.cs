using System;
using System.Collections.Generic;

namespace TravelNotes.Models;

public partial class articleOtherTags
{
    public int Id { get; set; }

    public int ArticleId { get; set; }

    public int OtherTagId { get; set; }

    public virtual article Article { get; set; } = null!;

    public virtual OtherTags OtherTag { get; set; } = null!;
}
