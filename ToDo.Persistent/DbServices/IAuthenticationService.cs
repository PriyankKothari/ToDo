using Microsoft.AspNetCore.Identity;

namespace ToDo.Persistent.DbServices
{
    public interface IAuthenticationService
    {
        IdentityUser Authenticate(string username, string password, out string token);
    }
}