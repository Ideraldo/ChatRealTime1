using ChatAPI.Models;
using System.Collections.Concurrent;

namespace ChatAPI.DataService
{
    public class ShareDb
    {
        private readonly ConcurrentDictionary<string, UserConnection> _connections = new();
        public ConcurrentDictionary<string, UserConnection> connections => _connections;
    }
}
