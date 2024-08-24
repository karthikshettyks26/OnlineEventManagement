
namespace OnlineEventManagement.Repositories
{
    public class EventRegisterRepository : IEventRegisterRepository
    {
        private readonly IConfiguration configuration;

        public EventRegisterRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public Task<Guid?> AddEventRegisterAsync(Guid eventId, Guid createdBy)
        {
            throw new NotImplementedException();
        }
    }
}
