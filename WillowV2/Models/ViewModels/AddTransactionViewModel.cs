using System.ComponentModel.DataAnnotations;

namespace WillowV2.Models.ViewModels
{
    public class AddTransactionViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public float Amount { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        public string? Notes { get; set; } = null;
        //public string UserId { get; set; }
    }
}
