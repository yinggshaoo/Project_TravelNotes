using System;
using System.Collections.Generic;

namespace TravelNotes.Models;

public partial class OtherTags
{
    public int OtherTagId { get; set; }

    public string OtherTagName { get; set; } = null!;

    public virtual ICollection<articleOtherTags> articleOtherTags { get; set; } = new List<articleOtherTags>();
}
