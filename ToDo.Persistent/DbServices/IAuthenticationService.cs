using System.Threading.Tasks;
using ToDo.Persistent.DbObjects;

namespace ToDo.Persistent.DbServices
{
    public interface IAuthenticationService
    {
        Task<ApplicationUser> Authenticate(string username, string password);
    }
}