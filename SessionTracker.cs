using System.Collections.Concurrent;

namespace WebApplication1
{
    public class SessionTracker
    {
        private readonly ConcurrentDictionary<string, string> _activeSessions = new();

        public void AddSession(string userId, string sessionId)
        {
            _activeSessions[userId] = sessionId;
        }

        public void RemoveSession(string userId)
        {
            _activeSessions.TryRemove(userId, out _);
        }

        public bool IsUserAlreadyLoggedIn(string userId)
        {
            return _activeSessions.ContainsKey(userId);
        }
    }
}
