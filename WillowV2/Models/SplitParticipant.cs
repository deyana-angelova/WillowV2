using System.ComponentModel.DataAnnotations.Schema;

namespace WillowProject.Models
{
    public class SplitParticipant
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountOwed { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; } = 0;
        //public bool IsPaid { get; set; } = false;

        // connection to splits
        public Guid SplitId { get; set; }
        public Split Split { get; set; }
    }
}
