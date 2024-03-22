using System;
using System.Collections.Generic;

namespace TravelNotes.Models;

public partial class article
{
    public int ArticleId { get; set; }

    public int UserId { get; set; }

    public string? Title { get; set; }

    public string? Subtitle { get; set; }

    public DateTime? PublishTime { get; set; }

    public DateTime? TravelTime { get; set; }

    public string? Contents { get; set; }

    public string? Images { get; set; }

    public int? LikeCount { get; set; }

    public int? PageView { get; set; }

    public string? ArticleState { get; set; }

    public int? SpotId { get; set; }

    public virtual users User { get; set; } = null!;

    public virtual ICollection<articleOtherTags> articleOtherTags { get; set; } = new List<articleOtherTags>();

    public virtual ICollection<messageBoard> messageBoard { get; set; } = new List<messageBoard>();
}
