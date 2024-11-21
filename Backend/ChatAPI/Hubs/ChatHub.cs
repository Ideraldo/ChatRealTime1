using ChatAPI.DataService;
using ChatAPI.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChatAPI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ShareDb _shared;

        public ChatHub(ShareDb shared) => _shared = shared;

        public async Task JoinChat(UserConnection connecttion)
        {
            await Clients.All
                .SendAsync("ReceiveMessage", "admin", $"{connecttion.UserName} has joined");
        }

        public async Task JoinSpecificChatRoom(UserConnection connecttion)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, connecttion.ChatRoom);

            _shared.connections[Context.ConnectionId] = connecttion;
            
            await Clients
                .Group(connecttion.ChatRoom)
                .SendAsync("JoinSpecificChatRoom", "admin", $"{connecttion.UserName} has joined {connecttion.ChatRoom}");
        }

        public async Task SendMessage(string msg)
        {
            if(_shared.connections.TryGetValue(Context.ConnectionId, out UserConnection connecttion))
            {
                await Clients.Groups(connecttion.ChatRoom)
                    .SendAsync("ReceiveSpecificMessage", connecttion.UserName, msg);
            }
        }

    }
}
