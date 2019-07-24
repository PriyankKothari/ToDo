using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo.Persistent.DbServices;

namespace ToDo.WebApi.Controllers
{
    /// <summary>
    /// User Controller
    /// </summary>
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/users")]
    [AllowAnonymous]
    public class UserController : Controller
    {
        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="authenticationService"></param>
        public UserController(IAuthenticationService authenticationService)
        {
            this._authenticationService = authenticationService;
        }
        
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] User user)
        {
            var authenticatedUser =
                this._authenticationService.Authenticate(user.Username, user.Password, out var token);

            if (authenticatedUser == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(token);
        }
    }

    public class User
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}