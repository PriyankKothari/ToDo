using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ToDo.Persistent.DbContexts;
using ToDo.Persistent.DbObjects;

namespace ToDo.Persistent.DbServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthorisationDbContext _authorisationDbContext;
        private readonly string _secret;

        //private const string Secret = "2BB80D537B1DA3E38BD30361AA855686BDE0EACD7162FEF6A25FE97BF527A25B";

        public AuthenticationService(UserManager<ApplicationUser> userManager,
            AuthorisationDbContext authorisationDbContext)
        {
            this._userManager = userManager;
            this._authorisationDbContext = authorisationDbContext;
            this._secret = "2BB80D537B1DA3E38BD30361AA855686BDE0EACD7162FEF6A25FE97BF527A25B";
        }

        public IdentityUser Authenticate(string username, string password, out string token)
        {
            token = string.Empty;

            var authenticatedUser =
                this._authorisationDbContext.Users.FirstOrDefault(us => us.UserName.Equals(username));

            if (authenticatedUser == null ||
                !_userManager.CheckPasswordAsync(authenticatedUser, password).Result) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var signingSecretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this._secret));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(signingSecretKey, SecurityAlgorithms.HmacSha256Signature)
            };

            token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

            return authenticatedUser;
        }
    }
}