using System.ComponentModel.DataAnnotations;

namespace WillowProject.Models
{
    public class AddSplitViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public string? Notes { get; set; } = null;

        // list of participants
        [MinLength(1, ErrorMessage = "At least one participant is required!")]
        public List<ParticipantViewModel> Participants { get; set; } = new List<ParticipantViewModel>();
        public class ParticipantViewModel
        {
            public string Name { get; set; }
            public decimal AmountOwed { get; set; }
            public decimal AmountPaid { get; set; }
        }
    }
}
