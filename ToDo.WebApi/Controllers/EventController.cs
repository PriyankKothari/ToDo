using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using ToDo.Persistent.DbObjects;
using ToDo.Persistent.DbServices;

namespace ToDo.WebApi.Controllers
{
    /// <summary>
    /// Event Controller
    /// </summary>
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/events")]
    //[Authorize]
    public class EventController : Controller
    {
        private readonly IEventStoreService _eventStoreService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventStoreService"></param>
        public EventController(IEventStoreService eventStoreService)
        {
            this._eventStoreService = eventStoreService;
        }

        /// <summary>
        /// Returns all events.
        /// </summary>
        /// <response code = "200">Returns all events.</response>
        /// <response code = "404">Returns NotFound with message: Events cannot be found.</response>
        /// <response code = "500">Returns InternalServerError with message: Something went wrong while getting all the events. Please try again in a while.</response>
        [HttpGet]
        [SwaggerOperation("get", Tags = new[] { "Events" })]
        [ProducesResponseType(typeof(Event), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _eventStoreService.GetEvents();
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError,
                    "Something went wrong while getting all the events. Please try again in a while.");
            }
        }

        /// <summary>
        /// Creates an event.
        /// </summary>
        /// <param name="event"></param>
        /// <response code = "201">Returns an event is created.</response>
        /// <response code = "400">Returns BadRequest with validation error message(s).</response>
        /// <response code = "404">Returns NotFound with message: Event cannot be found.</response>
        /// <response code = "500">Returns InternalServerError with message: Something went wrong while creating an event. Please try again in a while.</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Event @event)
        {
            try
            {
                var result = await this._eventStoreService.CreateEvent(@event);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError,
                    "Something went wrong while creating an event. Please try again in a while.");
            }
        }
    }
}