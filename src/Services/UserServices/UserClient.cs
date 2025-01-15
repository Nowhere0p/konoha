using System.Threading.Tasks;
using Konoha.Common;
using Konoha.Models;

namespace Konoha.Services;

public class UserClient(IConfiguration configuration, IMongoDbService<UserDetails> userDbService)
    : IUserClient
{
    private readonly IMongoDbService<UserDetails> _userDbService = userDbService;
    private readonly IConfiguration _configuration = configuration;

    public async Task<UserDetails> GetUserByIdAsync(string userId)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new KonohaException(KonohaException.BadRequest, "User Id is required");
            }
            var user = (
                await _userDbService.GetItemsAsync(u => u.UserId == userId)
            ).FirstOrDefault();
            if (user == null)
            {
                throw new KonohaException(KonohaException.NotFound, "User not found");
            }
            return user;
        }
        catch (KonohaException ex)
        {
            throw new KonohaException(KonohaException.InternalServerError, ex.Message);
        }
    }
}
