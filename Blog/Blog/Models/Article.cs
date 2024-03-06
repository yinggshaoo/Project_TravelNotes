using System;
using System.Collections.Generic;

namespace Blog.Models;

public partial class Article
{
    public int ArticleId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Subtitle { get; set; }

    public DateTime? PublishTime { get; set; }

    public DateTime? TravelTime { get; set; }

    public string? Contents { get; set; }

    public string? Location { get; set; }

    public string? Images { get; set; }

    public int? LikeCount { get; set; }

    public int? PageView { get; set; }

    public string? ArticleState { get; set; }

    public virtual ICollection<MessageBoard> MessageBoards { get; set; } = new List<MessageBoard>();

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public virtual User User { get; set; } = null!;
}
