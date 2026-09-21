using System.ComponentModel.DataAnnotations;
using WillowV2.Models.Enums;

namespace WillowV2.Models
{
    public class FinCategory
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public TransactionType Type { get; set; } //income or expense

        // connection to user
        public string UserId { get; set; }
        public User? User { get; set; }
    }
}
