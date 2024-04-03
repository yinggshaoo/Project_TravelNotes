using System.Collections;
using System.Collections.Generic;

namespace TravelNotes.Models
{
	public class UsersArticleViewModel
	{
		public users users { get; set; }

		public int UserId { get; set; }

		public string UserName { get; set; } = null!;

		public string? Phone { get; set; }

		public string? Mail { get; set; }

		public string? Gender { get; set; }

		public string? Pwd { get; set; }

		public string? Nickname { get; set; }

		public DateOnly? Birthday { get; set; }

		public string? Address { get; set; }

		public string? Introduction { get; set; }

		public string? Interest { get; set; }

		public string? Headshot { get; set; }

		public string? SuperUser { get; set; }


		public virtual ICollection<album> album { get; set; } = new List<album>();

		public virtual ICollection<messageBoard> messageBoard { get; set; } = new List<messageBoard>();

		public virtual ICollection<photo> photo { get; set; } = new List<photo>();

		public IEnumerable<article> article { get; set; }

		public int ArticleId { get; set; }

		public string? Title { get; set; }

		public string? Subtitle { get; set; }

		public DateTime? PublishTime { get; set; }

		public DateTime? TravelTime { get; set; }

		public string? Contents { get; set; }

		public string? Images { get; set; }

		public int? LikeCount { get; set; }

		public int? PageView { get; set; }

		public string? ArticleState { get; set; }

		public virtual users User { get; set; } = null!;

		public string PasswordCookie { get; set; }

		public List<int>? OtherTagIds { get; set; }

		public UsersArticleViewModel()
		{
		}
		public UsersArticleViewModel(IEnumerable<article> article)
		{
			article = article;
		}
	}

}
