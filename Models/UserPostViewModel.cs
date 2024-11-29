namespace StorySite.Models
{
    public class UserPostViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
        public string Type { get; set; } // "Prompt" or "Story"
    }
}
