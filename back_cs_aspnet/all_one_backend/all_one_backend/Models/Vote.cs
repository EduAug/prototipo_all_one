namespace all_one_backend.Models
{
    public class Vote
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int TopicId { get; set; }
        public Topic Topic { get; set; } = null!;
        public DateTime VoteDate { get; set; }
    }
}
