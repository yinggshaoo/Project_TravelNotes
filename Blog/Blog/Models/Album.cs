using System;
using System.Collections.Generic;

namespace Blog.Models;

public partial class Album
{
    public int AlbumId { get; set; }

    public int UserId { get; set; }

    public string AlbumName { get; set; } = null!;

    public DateOnly? CreateTime { get; set; }

    public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();

    public virtual User User { get; set; } = null!;
}
