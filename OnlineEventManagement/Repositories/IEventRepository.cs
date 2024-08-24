using Microsoft.AspNetCore.Mvc;
using OnlineEventManagement.Models.Domain;

namespace OnlineEventManagement.Repositories
{
    public interface IEventRepository
    {
        Task<Guid> AddEventAsync(Event @event);
        Task<IEnumerable<Event>> GetAllEventsAsync(string? filterOn, string? filterQuery, string? sortBy, bool? isAscending = true);
        Task<Event> GetEventByIdAsync(Guid eventId);
        Task<bool> UpdateEventAsync(Event @event, Guid Id);
        Task<bool> DeleteEventAsync(Guid eventId);

    }
}
