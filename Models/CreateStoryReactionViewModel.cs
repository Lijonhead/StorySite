namespace StorySite.Models
{
    public class CreateStoryReactionViewModel
    {
        public string Reaction { get; set; } // "Like" or "Dislike"
        public int StoryId { get; set; }
        public string UserId { get; set; }
    }
}
