using System.ComponentModel.DataAnnotations;

namespace WebApplication1
{
	public class AllowedExtensions : ValidationAttribute
	{
		private readonly string[] _extensions;

		public AllowedExtensions(string[] extensions)
		{
			_extensions = extensions;
		}

		protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
		{
			var file = value as IFormFile;
			if (file != null)
			{
				var extension = Path.GetExtension(file.FileName).ToLower();
				if (!_extensions.Contains(extension))
				{
					return new ValidationResult("Only .pdf and .docx files are allowed");
				}
			}
			return ValidationResult.Success;
		}
	}
}
