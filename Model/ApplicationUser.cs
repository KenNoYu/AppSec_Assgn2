using Microsoft.AspNetCore.Identity;
using System;

namespace WebApplication1.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string NRIC { get; set; }
        public DateTime DateOfBirth { get; set; }
        public byte[] Resume { get; set; }
        public string ResumeFileName { get; set; }
        public string ResumeContentType { get; set; }
        public string WhoAmI { get; set; }
        public List<string> PreviousPasswords { get; set; } = new();
        public DateTime LastPasswordChange { get; set; } = DateTime.UtcNow;
    }
}
