using Microsoft.AspNetCore.Identity;

namespace ToDo.Persistent.DbObjects
{
    public class ApplicationUser : IdentityUser
    {
        public string Token { get; set; }
    }
}