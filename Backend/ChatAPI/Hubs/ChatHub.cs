using ChatAPI.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChatAPI.Hubs
{
    public class ChatHub : Hub
    {
        public async Task JoinChat(UserConnection connect)
        {
            await Clients.All
                .SendAsync("ReceiveMessage", "admin", $"{connect.UserName} has joined");
        }

        public async Task JoinSpecificChatRoom(UserConnection connect)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, connect.ChatRoom);
            await Clients
                .Group(connect.ChatRoom)
                .SendAsync("JoinSpecificChatRoom", "admin", $"{connect.UserName} has joined {connect.ChatRoom}");
        }
    }
}
