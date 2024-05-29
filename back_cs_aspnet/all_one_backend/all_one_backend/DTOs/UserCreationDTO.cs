namespace all_one_backend.DTOs
{
    public class UserCreationDTO
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime Birthday { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public List<int> TopicIds { get; set; }
    }
}
