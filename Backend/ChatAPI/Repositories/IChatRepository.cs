using System.Collections.Generic;
using System.Threading.Tasks;
using ChatAPI.Models;

namespace ChatAPI.Repositories
{
    public interface IChatRepository
    {
        Task AddMessageAsync(ChatMessage message);
        Task<IEnumerable<ChatMessage>> GetMessagesByChatRoomAsync(string chatRoom, int limit = 50);
    }
}