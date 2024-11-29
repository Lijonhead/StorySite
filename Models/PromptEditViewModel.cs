namespace StorySite.Models
{
    public class PromptEditViewModel
    {
        public int Id { get; set; }
        public string PromptContent { get; set; }
        public string UserId { get; set; } // Ensure this matches the API definition
    }
}
