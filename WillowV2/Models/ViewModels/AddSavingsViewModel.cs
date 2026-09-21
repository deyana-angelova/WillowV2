using System.ComponentModel.DataAnnotations;

namespace WillowV2.Models.ViewModels
{
    public class AddSavingsViewModel
    {
        [Required]
        public float AmountToAdd { get; set; }
        [Required]
        public float AmountToRemove { get; set; }
    }
}
