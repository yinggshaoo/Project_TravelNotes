using System;
using System.Collections.Generic;

namespace Blog.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Mail { get; set; }

    public string? Gender { get; set; }

    public string? Pwd { get; set; }

    public string? Nickname { get; set; }

    public DateTime? Birthday { get; set; }

    public string? Address { get; set; }

    public string? Introduction { get; set; }

    public string? Interest { get; set; }

    public string? Haedshot { get; set; }

    public string? SuperUser { get; set; }

    public virtual ICollection<Album> Albums { get; set; } = new List<Album>();

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual ICollection<MessageBoard> MessageBoards { get; set; } = new List<MessageBoard>();
}
