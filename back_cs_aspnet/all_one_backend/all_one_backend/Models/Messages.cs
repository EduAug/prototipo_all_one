using System;
using System.Collections.Generic;

namespace all_one_backend.Models;

public class Messages
{
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public Guid MessageId { get; set; }
    public string MessageText { get; set; }
    public DateTime Timestamp { get; set; }
}
