namespace StorySite.Models
{
    public class ProfileViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public string Description { get; set; }
        public string Picture { get; set; }
        public string UserId { get; set; }
        public DateOnly ProfileCreated { get; set; }
    }
}
