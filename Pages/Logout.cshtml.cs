using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;

namespace WebApplication1.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AuditLogService _auditLogService;
        private readonly SessionTracker _sessionTracker;
        private readonly UserManager<ApplicationUser> _userManager;

        public LogoutModel(
            SignInManager<ApplicationUser> signInManager,
            AuditLogService auditLogService,
            SessionTracker sessionTracker,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _auditLogService = auditLogService;
            _sessionTracker = sessionTracker;
            _userManager = userManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var userId = user.Id;
                _sessionTracker.RemoveSession(userId);
                await _auditLogService.LogActivityAsync(userId, "User logged out");
                await _signInManager.SignOutAsync();
                HttpContext.Session.Clear();
            }
            return RedirectToPage("/Login");
        }
    }
}
