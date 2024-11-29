namespace StorySite.Models
{
    public class PromptReactionViewModel
    {
        public int Id { get; set; }
        public string Reaction { get; set; }  // "Like" or "Dislike"
        public int PromptId { get; set; }
        public string UserId { get; set; }
    }
}
