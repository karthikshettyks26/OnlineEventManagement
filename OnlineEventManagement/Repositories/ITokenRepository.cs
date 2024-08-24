using Microsoft.AspNetCore.Identity;

namespace OnlineEventManagement.Repositories
{
    public interface ITokenRepository
    {
        string CreateJwtToken(IdentityUser user, List<String> roles);
    }
}
