using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;

namespace WebApplication1.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        [BindProperty]
        public ForgotPasswordInputModel Input { get; set; }

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public void OnGet()
        {
        }

		[ValidateAntiForgeryToken]
		public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
				System.Diagnostics.Debug.WriteLine($"{user}");
				if (user == null)
                {
					// Don't reveal that the user does not exist or is not confirmed
					System.Diagnostics.Debug.WriteLine($"User don't exists");
					return RedirectToPage("/ForgotPasswordConfirmation");
                }

                // Generate password reset token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page("/ChangeForgetPassword", pageHandler: null, values: new { token, email = user.Email }, protocol: Request.Scheme);

				// Send email
				System.Diagnostics.Debug.WriteLine($"Sending Email");
				await _emailService.SendEmailAsync("tanjy0206@gmail.com", "Reset Password", $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>.");


				return RedirectToPage("/ForgotPasswordConfirmation");
            }
            return Page();
        }
    }

    public class ForgotPasswordInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
