using System.Threading.Tasks;
using Konoha.Models;

namespace Konoha.Services;

public interface IUserClient
{
    Task<UserDetails> GetUserByIdAsync(string userId);
}
