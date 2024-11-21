using ChatAPI.Models;
using System.Collections.Concurrent;

namespace ChatAPI.DataService
{
    public class ShareDb
    {
        public ConcurrentDictionary<string, UserConnection> Connections { get; } = new();
    }
}
