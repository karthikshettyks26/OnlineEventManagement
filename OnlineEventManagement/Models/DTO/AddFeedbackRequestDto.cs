using System.ComponentModel.DataAnnotations;

namespace OnlineEventManagement.Models.DTO
{
    public class AddFeedbackRequestDto
    {
        [Range(1,5)]
        [Required]
        public int Rating { get; set; }
        [Required]
        public string comment { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
