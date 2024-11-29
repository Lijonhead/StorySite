namespace StorySite.Models
{
    public class StoryViewModel
    {
        public int Id { get; set; }
        public string StoryContent { get; set; }
        public DateTime StoryDateCreated { get; set; }
        public int PromptId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }


    }

}
