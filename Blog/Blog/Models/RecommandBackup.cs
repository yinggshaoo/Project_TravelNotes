using System;
using System.Collections.Generic;

namespace Blog.Models;

public partial class RecommandBackup
{
    public string UserId { get; set; } = null!;

    public string? Gender { get; set; }

    public string? Age { get; set; }

    public string? LikeWeather { get; set; }

    public string? Interest1 { get; set; }

    public string? Interest2 { get; set; }

    public string? Interest3 { get; set; }

    public string? LikeLocation { get; set; }
}
