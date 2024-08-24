namespace OnlineEventManagement.Repositories
{
    public interface IEventRegisterRepository
    {
        Task<Guid?> AddEventRegisterAsync(Guid eventId, Guid createdBy);
    }
}
