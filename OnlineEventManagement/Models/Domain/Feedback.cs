namespace OnlineEventManagement.Models.Domain
{
    public class Feedback
    {
        public Guid Id { get; set; }
        public int Rating {  get; set; }
        public string comment { get; set; }
        public Guid EventId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
