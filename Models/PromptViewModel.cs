namespace StorySite.Models
{
    public class PromptViewModel
    {
        public int Id { get; set; }
        public string PromptContent { get; set; }
        public DateTime PromptDateCreated { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
