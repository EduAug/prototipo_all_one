using System;
using System.Collections.Generic;

namespace all_one_backend.Models;

public partial class User
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public DateTime Birthday { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public virtual ICollection<User> Friends { get; set; } = new List<User>();
    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
