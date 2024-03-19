using System;
using System.Collections.Generic;

namespace TravelNotes.Models;

public partial class myFavor
{
    public int SpotId { get; set; }

    public int UserId { get; set; }

    public virtual Spots Spot { get; set; } = null!;

    public virtual users User { get; set; } = null!;
}
