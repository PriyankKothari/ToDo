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
        public IActionResult Authenticate([FromBody] User userParam)
        {
            var user = this._authenticationService.Authenticate(userParam.Username, userParam.Password).Result;

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }
    }

    public class User
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}