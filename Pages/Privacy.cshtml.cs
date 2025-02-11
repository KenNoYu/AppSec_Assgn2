using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;

namespace WebApplication1.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public PrivacyModel(ILogger<PrivacyModel> logger, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
        }

        public async Task OnGetAsync()
        {
            // Check if the session is expired
            if (!HttpContext.Session.Keys.Contains("UserId"))
            {
                await _signInManager.SignOutAsync();
                HttpContext.Session.Clear();
                // Redirect to login page if session is expired
                Response.Redirect("/Login");
                return;
            }
        }
    }

}
