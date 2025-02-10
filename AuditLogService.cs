using WebApplication1.Model;
using WebApplication1.ViewModels;

namespace WebApplication1
{
    public class AuditLogService
    {
        private readonly AuthDbContext _context;

        public AuditLogService(AuthDbContext context)
        {
            _context = context;
        }

        public async Task LogActivityAsync(string userId, string activity)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Activity = activity,
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
