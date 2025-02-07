using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class Register
    {
        [Required] public string FirstName { get; set; }

        [Required] public string LastName { get; set; }

        [Required] public string Gender { get; set; }

        [Required] public string NRIC { get; set; }

        [Required][DataType(DataType.EmailAddress)] public string Email { get; set; }

        [Required][DataType(DataType.Password)] public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }

        [Required][DataType(DataType.Date)] public string DateOfBirth { get; set; }

        [Required][DataType(DataType.Upload)]public IFormFile Resume { get; set; }

        [Required][RegularExpression(@"^[^<>]*$", ErrorMessage = "Who Am I field allows all special characters except '<' and '>'.")]public string WhoAmI { get; set; }
    }

}
