using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatAPI.Models;
using ChatAPI.Repositories;

namespace ChatAPI.Services
{
    public class ChatService
    {
        private readonly IChatRepository _chatRepository;
        private static readonly Dictionary<string, string> Users = new Dictionary<string, string>();

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public bool AddUserToList(string userToAdd)
        {
            lock (Users)
            {
                if (Users.ContainsKey(userToAdd.ToLower()))
                    return false;

                Users.Add(userToAdd.ToLower(), null);
                return true;
            }
        }

        public async Task SaveMessageAsync(string userName, string chatRoom, string message)
        {
            var chatMessage = new ChatMessage
            {
                UserName = userName,
                ChatRoom = chatRoom,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            await _chatRepository.AddMessageAsync(chatMessage);
        }

        public async Task<IEnumerable<ChatMessage>> GetRecentMessagesAsync(string chatRoom, int limit = 50)
        {
            return await _chatRepository.GetMessagesByChatRoomAsync(chatRoom, limit);
        }
    }
}