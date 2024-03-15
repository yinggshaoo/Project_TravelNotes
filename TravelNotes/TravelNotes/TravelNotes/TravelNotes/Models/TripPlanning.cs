using System;
using System.Collections.Generic;

namespace TravelNotes.Models;

public partial class TripPlanning
{
    public int TripId { get; set; }

    public int UserId { get; set; }

    public string ScenicSpotID { get; set; } = null!;

    public virtual Spots ScenicSpot { get; set; } = null!;

    public virtual users User { get; set; } = null!;
}
