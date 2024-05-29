using System;
using System.Collections.Generic;

namespace all_one_backend.Models;

public partial class Topic
{
    public int Id { get; set; }
    public string TopicName { get; set; } = null!;
    public int Subscribers { get; set; }
    public int TotalVotes { get; set; }
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
