using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ToDo.Persistent.DbContexts;
using ToDo.Persistent.DbObjects;

namespace ToDo.Persistent.DbServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthorisationDbContext _authorisationDbContext;

        private const string Secret = "2BB80D537B1DA3E38BD30361AA855686BDE0EACD7162FEF6A25FE97BF527A25B";

        public AuthenticationService(UserManager<ApplicationUser> userManager, AuthorisationDbContext authorisationDbContext)
        {
            this._userManager = userManager;
            this._authorisationDbContext = authorisationDbContext;
        }
        public async Task<ApplicationUser> Authenticate(string username, string password)
        {
            var user = await this._authorisationDbContext.Users.FirstOrDefaultAsync(us => us.UserName.Equals(username),
                CancellationToken.None);// this._userManager.FindByNameAsync(username);

            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(Secret); //
                var signingKey = new SymmetricSecurityKey(key);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Email, user.Id)
                    }),
                    Expires = DateTime.UtcNow.AddSeconds(10),
                    SigningCredentials =
                        new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                user.Token = tokenHandler.WriteToken(token);
            }

            return user;
        }
    }
}