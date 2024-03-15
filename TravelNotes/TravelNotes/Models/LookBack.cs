using System;
using System.Collections.Generic;

namespace TravelNotes.Models;

public partial class LookBack
{
    public int Yid { get; set; }

    public int UserId { get; set; }

    public int PhotoId { get; set; }

    public virtual photo Photo { get; set; } = null!;

    public virtual users User { get; set; } = null!;
}
