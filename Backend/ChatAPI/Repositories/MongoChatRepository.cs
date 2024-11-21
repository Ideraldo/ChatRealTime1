using System.Collections.Generic;
using System.Threading.Tasks;
using ChatAPI.Models;
using MongoDB.Driver;

namespace ChatAPI.Repositories;

public class MongoChatRepository(IMongoDatabase database) : IChatRepository
{
    private readonly IMongoCollection<ChatMessage> _messages = database.GetCollection<ChatMessage>("messages");

    public async Task AddMessageAsync(ChatMessage message)
    {
        await _messages.InsertOneAsync(message);
    }

    public async Task<IEnumerable<ChatMessage>> GetMessagesByChatRoomAsync(string chatRoom, int limit = 50)
    {
        return await _messages
            .Find(m => m.ChatRoom == chatRoom)
            .SortByDescending(m => m.Timestamp)
            .Limit(limit)
            .ToListAsync();
    }
}