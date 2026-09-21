using System.ComponentModel.DataAnnotations;

namespace WillowV2.Models.ViewModels
{
    public class AddGoalViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public float AmountToReach { get; set; }
        [Required]
        public float CurrentAmount { get; set; }
        [Required]
        public DateTime CurrentDate { get; set; }
        [Required]
        public DateTime Deadline { get; set; }
        public string? Notes { get; set; } = null;
    }
}
