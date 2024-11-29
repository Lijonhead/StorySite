namespace StorySite.Models
{
    public class CreateStoryViewModel
    {
        public int Id { get; set; } // Story ID (not used during creation but might be relevant later)
        public string StoryContent { get; set; }
        public DateTime StoryDateCreated { get; set; } = DateTime.UtcNow; // Automatically set
        public int PromptId { get; set; }
        public string UserId { get; set; }
    }
}
