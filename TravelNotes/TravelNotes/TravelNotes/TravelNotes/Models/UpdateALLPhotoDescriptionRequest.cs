namespace TravelNotes.Models
{
    public class UpdateALLPhotoDescriptionRequest
    {
        public List<int> PhotoIds { get; set; }
        public string NewDescription { get; set; }
    }
}
