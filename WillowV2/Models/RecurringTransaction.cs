using System.ComponentModel.DataAnnotations;
using WillowV2.Models.Enums;

namespace WillowV2.Models
{
    public class RecurringTransaction
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public float Amount { get; set; }
        [Required]
        public int DayOfMonth { get; set; } // 1-28, the day it repeats
        public Guid CategoryId { get; set; }
        public FinCategory? Category { get; set; }
        //public TransactionType Type { get; set; }
        public string UserId { get; set; }
        public DateTime? LastProcessed { get; set; } // when it was last applied
    }
}
