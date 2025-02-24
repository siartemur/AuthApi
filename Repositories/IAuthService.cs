using System.Threading.Tasks;

namespace AuthApi.Repositories
{
    public interface IAuthService
    {
        Task<string> GenerateJwtToken(string email);
    }
}
