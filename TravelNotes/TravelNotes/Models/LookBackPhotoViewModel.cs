using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TravelNotes.Models
{
	public class LookBackPhotoViewModel
	{
		public List<LookBack> LookBack { get; set; }
		public List<photo> photo { get; set; }
		public List<LookBackPhotoViewModel2> photoPaths { get; set; }
	}
}
