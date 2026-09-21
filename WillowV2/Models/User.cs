using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WillowV2.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string DisplayName { get; set; }
    }
}
