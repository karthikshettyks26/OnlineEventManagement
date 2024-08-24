using System.ComponentModel.DataAnnotations;

namespace OnlineEventManagement.Models.Domain
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime EventDate { get; set; }
        public int MaxParticipants { get; set; }
    }
}
