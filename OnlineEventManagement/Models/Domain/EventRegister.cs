namespace OnlineEventManagement.Models.Domain
{
    public class EventRegister
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
