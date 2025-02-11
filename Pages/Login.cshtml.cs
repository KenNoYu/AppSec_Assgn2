using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Model;

namespace WebApplication1.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuditLogService _auditLogService;
        private readonly SessionTracker _sessionTracker;
        private readonly ReCaptchaService _reCaptchaService;

        [BindProperty]
        public LoginInputModel Input { get; set; }

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            AuditLogService auditLogService,
            SessionTracker sessionTracker,
            ReCaptchaService reCaptchaService)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
            _auditLogService = auditLogService;
            _sessionTracker = sessionTracker;
            _reCaptchaService = reCaptchaService;
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var reCaptchaToken = Request.Form["g-recaptcha-response"];
                var isHuman = await _reCaptchaService.VerifyToken(reCaptchaToken);

                if (!isHuman)
                {
                    ModelState.AddModelError(string.Empty, "reCAPTCHA validation failed. Please try again.");
                    return Page();
                }

                var user = await _userManager.FindByEmailAsync(Input.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Email or Password.");
                    await _auditLogService.LogActivityAsync("UNKNOWN_USER", $"Failed login attempt: {Input.Email}");
                    return Page();
                }

                // Ensure lockout is enabled for the user
                if (!user.LockoutEnabled)
                {
                    await _userManager.SetLockoutEnabledAsync(user, true);
                    await _userManager.UpdateAsync(user);
                }

                // If the user is already locked out, prevent further login attempts
                if (await _userManager.IsLockedOutAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "Your account is locked due to multiple failed login attempts. Please try again later.");
                    await _auditLogService.LogActivityAsync(user.Id, $"Account already locked: {Input.Email}");
                    return Page();
                }

                // Check if the user is already logged in from another device
                if (_sessionTracker.IsUserAlreadyLoggedIn(user.Id))
                {
                    ModelState.AddModelError(string.Empty, "User is already logged in from another device.");
                    await _auditLogService.LogActivityAsync(user.Id, "Failed login attempt: User already logged in");
                    return Page();
                }

                // Attempt to sign in
                var result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, false, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // Set session variable to track the user's session
                    HttpContext.Session.SetString("UserId", user.Id); // Add this line
                    await HttpContext.Session.CommitAsync();

                    // Add the session to the tracker
                    _sessionTracker.AddSession(user.Id, HttpContext.Session.Id);
                    await _auditLogService.LogActivityAsync(user.Id, "User logged in");
                    await _userManager.ResetAccessFailedCountAsync(user);

                    return RedirectToPage("/Home");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Account locked out due to multiple failed login attempts. Please try again later.");
                    await _auditLogService.LogActivityAsync(user.Id, $"Account locked out: {Input.Email}");
                }
                else
                {
                    // Reload user to ensure latest failed count is used
                    user = await _userManager.FindByIdAsync(user.Id);
                    var lockCount = await _userManager.GetAccessFailedCountAsync(user);

                    ModelState.AddModelError(string.Empty, "Invalid Email or Password.");
                    await _auditLogService.LogActivityAsync(user.Id, $"Failed login attempt: {Input.Email}");

                    if (lockCount >= 3)
                    {
                        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddMinutes(1)); // Lockout for 1 minute
                        await _userManager.UpdateAsync(user);
                    }
                }
            }
            return Page();
        }

    }
    public class LoginInputModel
    {
        [Required][DataType(DataType.EmailAddress)] public string Email { get; set; }

        [Required][DataType(DataType.Password)] public string Password { get; set; }
    }
}
