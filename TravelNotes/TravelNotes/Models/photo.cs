using System;
using System.Collections.Generic;

namespace TravelNotes.Models;

public partial class photo
{
    public int PhotoId { get; set; }

    public string PhotoTitle { get; set; } = null!;

    public string? PhotoDescription { get; set; }

    public string? PhotoPath { get; set; }

    public DateOnly? UploadDate { get; set; }

    public int? AlbumId { get; set; }

    public int? UserId { get; set; }

    public virtual album? Album { get; set; }

    public virtual users? User { get; set; }
}
