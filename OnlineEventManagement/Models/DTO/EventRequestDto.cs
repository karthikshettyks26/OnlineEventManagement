using System.ComponentModel.DataAnnotations;

namespace OnlineEventManagement.Models.DTO
{
    public class EventRequestDto
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime EventDate { get; set; }
        [Required]
        public int MaxParticipants { get; set; }
    }
}
