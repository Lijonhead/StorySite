namespace StorySite.Models
{
    public class PromptDetailsViewModel
    {
        public int Id { get; set; }
        public string PromptContent { get; set; }
        public DateTime PromptDateCreated { get; set; }
        public string UserName { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public string UserId { get; set; }

        public List<StoryViewModel> Stories { get; set; } = new List<StoryViewModel>();
    }
}
