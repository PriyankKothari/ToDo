using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ToDo.UI.Controllers
{
    [Route("api/users")]
    public class SampleDataController : Controller
    {
        private HttpClient _apiClient = new HttpClient();

        [HttpPost("authenticate/{username}/{password}")]
        public async Task<IActionResult> Authenticate(string username, string password)
        {
            using (this._apiClient = new HttpClient())
            {
                this._apiClient.BaseAddress = new Uri("http://localhost:44320/v1.0/");

                HttpResponseMessage response = await this._apiClient.PostAsJsonAsync("users/authenticate",
                    new User {Username = username, Password = password});

                var responseJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return Ok(JsonConvert.DeserializeObject<User>(responseJson));
                }
            }
            // return URI of the created resource.
            return Ok();
        }
    }

    public class User
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }
    }
}
