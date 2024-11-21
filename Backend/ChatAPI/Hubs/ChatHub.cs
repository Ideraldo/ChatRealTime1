using System;
using ChatAPI.DataService;
using ChatAPI.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using ChatAPI.Services;
using ChatAPI.Constants;

namespace ChatAPI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ShareDb _shared;
        private readonly ChatService _chatService;
        private readonly OllamaService _ollamaService;

        public ChatHub(ShareDb shared, ChatService chatService, OllamaService ollamaService)
        {
            _shared = shared;
            _chatService = chatService;
            _ollamaService = ollamaService;
        }

        public async Task JoinChat(UserConnection connection)
        {
            await Clients.All
                .SendAsync(HubConstants.RECEIVE_MESSAGE, UserConstants.SYSTEM_USER,
                    $"{connection.UserName} has joined");
        }

        public async Task JoinSpecificChatRoom(UserConnection connection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);

            _shared.Connections[Context.ConnectionId] = connection;

            await Clients
                .Group(connection.ChatRoom)
                .SendAsync(HubConstants.JOIN_SPECIFIC_CHAT_ROOM, UserConstants.SYSTEM_USER,
                    $"{connection.UserName} has joined {connection.ChatRoom}");

            // Adicionar Ollama ao grupo
            // VALIDAR SE ELE JA ESTA NO GRUPO
            await Clients
                .Group(connection.ChatRoom)
                .SendAsync(HubConstants.JOIN_SPECIFIC_CHAT_ROOM, UserConstants.OLLAMA_USER,
                    "Ollama AI has joined the chat");
        }

        public async Task SendMessage(string msg)
        {
            if (_shared.Connections.TryGetValue(Context.ConnectionId, out UserConnection connection))
            {
                await _chatService.SaveMessageAsync(connection.UserName, connection.ChatRoom, msg);
                await Clients.Group(connection.ChatRoom)
                    .SendAsync("ReceiveSpecificMessage", connection.UserName, msg);

                _ollamaService.AddMessageToContext(connection.UserName, msg);

                if (_ollamaService.ShouldRespond())
                {
                    await SendOllamaResponse(connection.ChatRoom);
                }
            }
        }

        public async Task GetRecentMessages(string chatRoom)
        {
            var recentMessages = await _chatService.GetRecentMessagesAsync(chatRoom);
            await Clients.Caller.SendAsync(HubConstants.RECEIVE_RECENT_MESSAGES, recentMessages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (_shared.Connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _shared.Connections.TryRemove(Context.ConnectionId, out _);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userConnection.ChatRoom);
                await Clients.Group(userConnection.ChatRoom).SendAsync(HubConstants.RECEIVE_MESSAGE,
                    UserConstants.SYSTEM_USER, $"{userConnection.UserName} left the chat");
            }

            await base.OnDisconnectedAsync(exception);
        }

        private async Task SendOllamaResponse(string chatRoom)
        {
            await foreach (var response in _ollamaService.GenerateResponseAsync())
            {
                await _chatService.SaveMessageAsync("Ollama AI", chatRoom, response);
                await Clients.Group(chatRoom)
                    .SendAsync("ReceiveSpecificMessage", "Ollama AI", response);

                _ollamaService.AddMessageToContext("Ollama AI", response);

                // Pequena pausa entre mensagens para simular digitação
                await Task.Delay(1000);
            }
        }
    }
}