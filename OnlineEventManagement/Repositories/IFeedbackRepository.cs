using OnlineEventManagement.Models.Domain;

namespace OnlineEventManagement.Repositories
{
    public interface IFeedbackRepository
    {
        Task<Guid?> AddFeedbackAsync(Feedback feedback);
    }
}
