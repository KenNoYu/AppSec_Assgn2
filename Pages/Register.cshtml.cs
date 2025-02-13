using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.ViewModels;
using WebApplication1.Model;
using System.Web;
using System;

namespace WebApplication1.Pages
{
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private readonly ReCaptchaService _reCaptchaService;
        private readonly IWebHostEnvironment _environment;


        [BindProperty]
        public Register RModel { get; set; }

        public RegisterModel
            (UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            ReCaptchaService reCaptchaService,
            IWebHostEnvironment environment)
        {
            this.userManager = userManager; 
            this.signInManager = signInManager; 
            _reCaptchaService = reCaptchaService;
            _environment = environment;
        }

        public void OnGet()
        {
        }

        //Save data into the database
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

                if (RModel.ResumePath == null || RModel.ResumePath.Length == 0)
                {
                    ModelState.AddModelError("RModel.Resume", "Resume is required.");
                    return Page();
                }

                var user = new ApplicationUser()
                {
                    FirstName = EncryptionHelper.Encrypt(RModel.FirstName),
                    LastName = EncryptionHelper.Encrypt(RModel.LastName),
                    Gender = EncryptionHelper.Encrypt(RModel.Gender),
                    NRIC = EncryptionHelper.Encrypt(RModel.NRIC),
                    UserName = RModel.Email,
                    Email = RModel.Email,
                    DateOfBirth = Convert.ToDateTime(RModel.DateOfBirth),
                    WhoAmI = EncryptionHelper.Encrypt(HttpUtility.HtmlEncode(RModel.WhoAmI)),
                };
                var result = await userManager.CreateAsync(user, RModel.Password); 
                if (result.Succeeded)
                {
					var allowedExtensions = new[] { ".pdf", ".docx" };
					var extension = Path.GetExtension(RModel.ResumePath.FileName).ToLower();

					if (!allowedExtensions.Contains(extension))
					{
						ModelState.AddModelError("", "Only .pdf and .docx files are allowed");
						return Page();
					}

					var uploadsFolder = Path.Combine(_environment.WebRootPath, "Resume");
					Directory.CreateDirectory(uploadsFolder);
					var uniqueFileName = Guid.NewGuid().ToString() + extension;
					var filePath = Path.Combine(uploadsFolder, uniqueFileName);

					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						await RModel.ResumePath.CopyToAsync(fileStream);
					}

                    user.ResumePath = "/resumes/" + uniqueFileName;
                    await userManager.UpdateAsync(user);

					return RedirectToPage("/Login");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }   
            return Page();
        }

    }
}
