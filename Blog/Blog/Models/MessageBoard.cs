using System;
using System.Collections.Generic;

namespace Blog.Models;

public partial class MessageBoard
{
    public int MessageId { get; set; }

    public int ArticleId { get; set; }

    public int? UserId { get; set; }

    public string? Contents { get; set; }

    public DateOnly? MessageTime { get; set; }

    public virtual Article Article { get; set; } = null!;

    public virtual User? User { get; set; }
}
