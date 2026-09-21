using System.ComponentModel.DataAnnotations;
using WillowV2.Models.Enums;

namespace WillowV2.Models
{
    public class FinTransaction
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public float Amount { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public string? Notes { get; set; } = null;

        [Required]
        public Guid CategoryId { get; set; }
        public FinCategory? Category { get; set; } //navigation property
        // connection to user
        public string UserId { get; set; }  
    }
}
