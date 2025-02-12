using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;
using System.Reflection;
using System.Web;

namespace WebApplication1.Pages
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        [BindProperty]
        public ChangePasswordInputModel Input { get; set; }

        public ChangePasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
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
        }

		[ValidateAntiForgeryToken]
		public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Validate password history (check last 2 passwords)
                foreach (var oldPassword in user.PreviousPasswords.TakeLast(2))
                {
                    var result2 = _userManager.PasswordHasher.VerifyHashedPassword(user, oldPassword, Input.NewPassword);
                    if (result2 == PasswordVerificationResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "You cannot reuse your last 2 passwords.");
                        return Page();
                    }
                }

                // Check minimum password age (e.g., prevent changing password more than once per day)
                if ((DateTime.UtcNow - user.LastPasswordChange).TotalMinutes < 2)
                {
                    ModelState.AddModelError(string.Empty, "You can only change your password once every 2 minutes.");
                    return Page();
                }

                // Store current password hash before changing
                string oldPasswordHash = user.PasswordHash;

                // Change password
                var result = await _userManager.ChangePasswordAsync(user, Input.CurrentPassword, Input.NewPassword);
                if (result.Succeeded)
                {
                    // Update password history and last password change time
                    user.PreviousPasswords.Add(oldPasswordHash); // Store old password
                    if (user.PreviousPasswords.Count > 2)
                    {
                        user.PreviousPasswords.RemoveAt(0); // Keep only last 2 passwords
                    }
                    user.LastPasswordChange = DateTime.UtcNow;

                    await _userManager.UpdateAsync(user);
                    await _signInManager.RefreshSignInAsync(user);
                    return RedirectToPage("/Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }
    }

    public class ChangePasswordInputModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{12,}$",
        ErrorMessage = "Password must be at least 12 characters long, with at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}
