namespace StorySite.Models
{
    public class UpdatePromptReactionViewModel
    {
        public int Id { get; set; }
        public string Reaction { get; set; }
        public int PromptId { get; set; }
        public string UserId { get; set; }
    }
}
