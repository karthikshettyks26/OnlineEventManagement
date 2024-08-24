using OnlineEventManagement.Models.Domain;

namespace OnlineEventManagement.Repositories
{
    public interface IEventRepository
    {
        Task<Guid> AddEventAsync(Event @event);
        Task<IEnumerable<Event>> GetAllEvents();

    }
}
