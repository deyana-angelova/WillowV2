using WillowV2.Models;

namespace WillowProject.Models
{
    public class Split
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime Date { get; set; }
        public string? Notes { get; set; } = null;

        // connection to user
        public string UserId { get; set; }
        public User? User { get; set; }

        // list of participants
        public List<SplitParticipant> Participants { get; set; } = new List<SplitParticipant>();
    }
}
