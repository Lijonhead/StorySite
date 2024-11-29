namespace StorySite.Models
{
    public class UserPostHistoryViewModel
    {
        public string UserId { get; set; }
        public List<PromptViewModel> Prompts { get; set; }
        public List<StoryViewModel> Stories { get; set; }
    }
}
