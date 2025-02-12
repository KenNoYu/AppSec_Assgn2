using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;

namespace WebApplication1.Pages
{
    public class ChangeForgetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ChangeForgetPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public ResetPasswordInputModel Input { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }

        public class ResetPasswordInputModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(Token) || string.IsNullOrEmpty(Email))
            {
                return BadRequest("Invalid password reset request.");
            }
            return Page();
        }

		[ValidateAntiForgeryToken]
		public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return RedirectToPage("/ResetPasswordConfirmation");
            }

            string oldPasswordHash = user.PasswordHash;

			var result = await _userManager.ResetPasswordAsync(user, Token, Input.NewPassword);
            if (result.Succeeded)
            {
				// Update password history and last password change time
				user.PreviousPasswords.Add(oldPasswordHash); // Store old password
				if (user.PreviousPasswords.Count > 2)
				{
					user.PreviousPasswords.RemoveAt(0); // Keep only last 2 passwords
				}
				user.LastPasswordChange = DateTime.UtcNow;
				return RedirectToPage("/Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
