using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;
using System.Web;

namespace WebApplication1.Pages
{
    public class HomeModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Gender { get; private set; }
        public string NRIC { get; private set; }
        public string Email { get; private set; }
        public string DateOfBirth { get; private set; }
        public string WhoAmI { get; private set; }
        public string ResumePath { get; private set; }

        public HomeModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this._userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task OnGetAsync()
        {
            // Check if the session is expired
            if (!HttpContext.Session.Keys.Contains("UserId"))
            {
                // Redirect to login page if session is expired
                await _signInManager.SignOutAsync();
                HttpContext.Session.Clear();
                Response.Redirect("/Login");
                return;
            }

            var user = await _userManager.GetUserAsync(User);
            // check if password expired
            if ((DateTime.UtcNow - user.LastPasswordChange).TotalMinutes > 5)
            {
                TempData["PasswordExpired"] = "Your password has expired. Please change your password.";
                Response.Redirect("/ChangePassword");
                return;
            }

            if (user != null)
            {
                FirstName = EncryptionHelper.Decrypt(user.FirstName);
                LastName = EncryptionHelper.Decrypt(user.LastName);
                Gender = EncryptionHelper.Decrypt(user.Gender);
                NRIC = EncryptionHelper.Decrypt(user.NRIC);
                Email = user.Email;
                DateOfBirth = user.DateOfBirth.ToString("yyyy-MM-dd");
                WhoAmI = HttpUtility.HtmlDecode(EncryptionHelper.Decrypt(user.WhoAmI));
                ResumePath = user.ResumePath;
            }
        }
    }
}
