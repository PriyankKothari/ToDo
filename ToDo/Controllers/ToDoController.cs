using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ToDo.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ToDoController : Controller
    {
        public ToDoController()
        {
            
        }
    }
}