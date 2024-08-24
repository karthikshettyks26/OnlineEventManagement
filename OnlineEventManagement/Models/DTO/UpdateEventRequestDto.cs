namespace OnlineEventManagement.Models.DTO
{
    public class UpdateEventRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public int MaxParticipants { get; set; }
    }
}
