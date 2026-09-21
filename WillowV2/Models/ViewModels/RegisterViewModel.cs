using System.ComponentModel.DataAnnotations;

namespace WillowV2.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string DisplayName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "The password must be minimum 6 characters long!")]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage ="Passwords do not match!")]
        public string ConfirmPassword { get; set; }
    }
}
