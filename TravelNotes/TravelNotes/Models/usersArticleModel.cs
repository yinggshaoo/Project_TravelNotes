namespace TravelNotes.Models
{
    public class usersArticleModel
    {
        public users user {  get; set; }
        public article article { get; set; }

        public List<int> OtherTagIds { get; set; }

    }
}
