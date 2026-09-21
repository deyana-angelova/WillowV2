using System.ComponentModel.DataAnnotations;

namespace WillowV2.Models
{
    public class Savings
    {
        [Required]
        public Guid Id { get; set; }    
        [Required]
        public float CurrentAmount { get; set; }
        public string UserId { get; set; }
        public User? User { get; set; }
    }
}
