namespace StorySite.Models
{
    public class StoryReactionViewModel
    {
        public int Id { get; set; }
        public string Reaction { get; set; } // "Like" or "Dislike"
        public int StoryId { get; set; }
        public string UserId { get; set; }
    }
}
