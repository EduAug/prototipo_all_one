using all_one_backend.Models;
using Microsoft.AspNetCore.SignalR;

namespace all_one_backend
{
    public class ChatHub : Hub
    {
        private readonly Dictionary<int, string> _userConnections = new Dictionary<int, string>();
        private readonly Dictionary<string, List<Messages>> _groupMesssages = new Dictionary<string, List<Messages>>();

        public ChatHub(Dictionary<int,string> uConn, Dictionary<string, List<Messages>> gMess) 
        {
            _userConnections = uConn;
            _groupMesssages = gMess;
        }

        public List<Messages> getMessages(string fromThese) 
        {
            if (_groupMesssages.ContainsKey(fromThese))
            {
                return _groupMesssages[fromThese];
            }
            else
            {
                return new List<Messages>();
            }
        }
        public Dictionary<int, string> getConnections() { return _userConnections; }

        public override async Task OnConnectedAsync()
        {
            var userIdQueried = Context.GetHttpContext().Request.Query["userId"];
            if(!string.IsNullOrEmpty(userIdQueried) )
            {
                if (int.TryParse(userIdQueried, out var userId))
                {
                    _userConnections[userId] = Context.ConnectionId;
                }
            }
            await base.OnConnectedAsync();
        }

        public async Task JoinGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName)) return;
            if (!_groupMesssages.ContainsKey(groupName))
            {
                _groupMesssages[groupName] = new List<Messages>();
            }

            await Groups.AddToGroupAsync(Context.ConnectionId,groupName);
            //await Clients.Group(groupName).SendAsync("ReceiveMessage", "Introduction Bot" ,"Someone just joined!");
        }

        public async Task SendMessage(string groupName, string fromUserId, string toUserId, string message)
        {
            if (string.IsNullOrEmpty(groupName)) return;
            if (!int.TryParse(fromUserId, out int fromUserIdAsInt)) return;
            if (!int.TryParse(toUserId, out int toUserIdAsInt)) return;

            var messageToStore = new Messages
            {
                SenderId = fromUserIdAsInt,
                ReceiverId = toUserIdAsInt,
                MessageId = Guid.NewGuid(),
                MessageText = message,
                Timestamp = DateTime.UtcNow
            };

            if (!_groupMesssages.ContainsKey(groupName))
                _groupMesssages[groupName] = new List<Messages>();

            _groupMesssages[groupName].Add(messageToStore);

            if (_groupMesssages[groupName].Count > 200)
                _groupMesssages[groupName].RemoveRange(0, (_groupMesssages[groupName].Count - 200));

            await Clients.Group(groupName).SendAsync("ReceiveMessage", fromUserId, message);
        }
    }
}
