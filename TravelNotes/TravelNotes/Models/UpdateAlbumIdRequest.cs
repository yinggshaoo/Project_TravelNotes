namespace TravelNotes.Models
{
    public class UpdateAlbumIdRequest
    {
        public List<int> PhotoIds { get; set; }
        public DateOnly ReplaceTime { get; set; }
    }
}
