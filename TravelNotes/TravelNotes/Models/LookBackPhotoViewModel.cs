using System.ComponentModel.DataAnnotations;

namespace TravelNotes.Models
{
	public class LookBackPhotoViewModel
	{
		public IEnumerable<LookBack> LookBack { get; set; }

		public int Yid { get; set; }

		public int UserId { get; set; }

		public int PhotoId { get; set; }

		public virtual photo Photo { get; set; } = null!;

		public virtual users User { get; set; } = null!;

		public IEnumerable<photo> photo { get; set; }

		public string PhotoTitle { get; set; } = null!;

		public string? PhotoDescription { get; set; }

		public string? PhotoPath { get; set; }

		public DateOnly? UploadDate { get; set; }

		public int? AlbumId { get; set; }

		public virtual album? Album { get; set; }

		public LookBackPhotoViewModel()
		{
		}
		public LookBackPhotoViewModel(IEnumerable<LookBack> LookBack, IEnumerable<photo> photo)
		{
			LookBack = LookBack;
			photo = photo;
		}
	}
}
