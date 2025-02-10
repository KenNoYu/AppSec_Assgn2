using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.ViewModels;
using WebApplication1.Model;
using System;

namespace WebApplication1.Pages
{
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }

        [BindProperty]
        public Register RModel { get; set; }

        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager; this.signInManager = signInManager;
        }

        public void OnGet()
        {
        }

        //Save data into the database
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if (RModel.Resume == null || RModel.Resume.Length == 0)
                {
                    ModelState.AddModelError("RModel.Resume", "Resume is required.");
                    return Page();
                }

                using var memoryStream = new MemoryStream();
                await RModel.Resume.CopyToAsync(memoryStream);
                byte[] resumeBytes = memoryStream.ToArray(); // Convert IFormFile to byte[]

                var user = new ApplicationUser()
                {
                    FirstName = EncryptionHelper.Encrypt(RModel.FirstName),
                    LastName = EncryptionHelper.Encrypt(RModel.LastName),
                    Gender = EncryptionHelper.Encrypt(RModel.Gender),
                    NRIC = EncryptionHelper.Encrypt(RModel.NRIC),
                    UserName = RModel.Email,
                    Email = RModel.Email,
                    DateOfBirth = Convert.ToDateTime(RModel.DateOfBirth),
                    WhoAmI = EncryptionHelper.Encrypt(RModel.WhoAmI),
                    Resume = resumeBytes,
                    ResumeFileName = RModel.Resume.FileName,
                    ResumeContentType = RModel.Resume.ContentType
                };
                var result = await userManager.CreateAsync(user, RModel.Password); if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false); return RedirectToPage("Index");
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
