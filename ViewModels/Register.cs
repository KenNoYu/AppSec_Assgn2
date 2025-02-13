using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class Register
    {
        [Required]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "First name must contain only letters.")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "First name must contain only letters.")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"^(Male|Female|Other)$", ErrorMessage = "Gender must be 'Male', 'Female', or 'Other'.")]
        public string Gender { get; set; }

        [Required]
        [RegularExpression(@"^[STFG]\d{7}[A-JZ]$", ErrorMessage = "Invalid NRIC format.")]
        public string NRIC { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)] 
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{12,}$",
        ErrorMessage = "Password must be at least 12 characters long, with at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }

        [Required][DataType(DataType.Date)] public string DateOfBirth { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".pdf", ".docx" })]
        public IFormFile ResumePath { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "WhoAmI section must be under 500 characters.")]
        public string WhoAmI { get; set; }
    }

}
