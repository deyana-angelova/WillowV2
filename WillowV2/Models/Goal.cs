using System.ComponentModel.DataAnnotations;

namespace WillowV2.Models
{
    public class Goal
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public float AmountToReach { get; set; }
        [Required]
        public float CurrentAmount { get; set; }
        [Required]
        public DateTime CurrentDate { get; set; }
        [Required]
        public DateTime Deadline {  get; set; }
        public string? Notes { get; set; } = null;

        public string UserId { get; set; }
        public User? User { get; set; }
    }
}
