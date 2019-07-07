using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using ToDo.Persistent.DbEnums;
using ToDo.Persistent.DbObjects;
using ToDo.Persistent.DbServices;

namespace ToDo.WebApi.Controllers
{
    /// <summary>
    /// ToDo Controller
    /// </summary>
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/todos")]
    //    [Authorize]
    public class ToDoController : Controller
    {
        private readonly IToDoService _todoService;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="todoService"></param>
        /// <param name="userManager"></param>
        public ToDoController(IToDoService todoService, UserManager<IdentityUser> userManager)
        {
            this._todoService = todoService;
            this._userManager = userManager;
        }

        /// <summary>
        /// Returns all to-do Items for current user.
        /// </summary>
        /// <response code = "200">Returns all to-do items.</response>
        /// <response code = "404">Returns NotFound with message: To-do items cannot be found.</response>
        /// <response code = "500">Returns InternalServerError with message: Something went wrong while getting all the to-do items for {currentUser.UserName}. Please try again in a while..</response>
        [HttpGet]
        [SwaggerOperation("get", Tags = new[] { "ToDo Items" })]
        [ProducesResponseType(typeof(ToDoItem), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (currentUser == null)
                return Unauthorized();

            try
            {
                var result = await _todoService.GetItems(currentUser);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    $"Something went wrong while getting all the to-do items for {currentUser.UserName}. Please try again in a while.");
            }
        }

        /// <summary>
        /// Returns to-do items by status for current user.
        /// </summary>
        /// <param name="itemStatus"></param>
        /// <response code = "200">Returns to-do items by status.</response>
        /// <response code = "404">Returns NotFound with message: To-do items by status {itemStatus} cannot be found.</response>
        /// <response code = "500">Returns InternalServerError with message: Something went wrong while getting all the to-do items for {currentUser.UserName} by status {itemStatus}. Please try again in a while.</response>
        [HttpGet("{itemStatus:itemStatus}")]
        public async Task<IActionResult> GetByItemStatus(ToDoStatuses itemStatus)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (currentUser == null)
                return Unauthorized();

            try
            {
                var result = await _todoService.GetItemsByStatus(currentUser, itemStatus);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    $"Something went wrong while getting all the to-do items for {currentUser.UserName} by status {itemStatus}. Please try again in a while.");
            }
        }

        /// <summary>
        /// Returns single to-do item by Item Id.
        /// </summary>
        /// <param name="itemId"></param>
        /// <response code = "200">Returns a to-do item by item id.</response>
        /// <response code = "404">Returns NotFound with message: To-do item by id {itemId} cannot be found.</response>
        /// <response code = "500">Returns InternalServerError with message: Something went wrong while getting the to-do item for {currentUser.UserName} by Id {itemId}. Please try again in a while.</response>
        [HttpGet("{itemId:int}")]
        public async Task<IActionResult> GetByItemId(int itemId)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (currentUser == null)
                return Unauthorized();

            try
            {
                var result = await _todoService.GetItemByItemId(currentUser, itemId);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    $"Something went wrong while getting the to-do item for {currentUser.UserName} by Id {itemId}. Please try again in a while.");
            }
        }

        /// <summary>
        /// Creates to-do item for current user.
        /// </summary>
        /// <param name="toDoItem"></param>
        /// <response code = "201">Returns a to-do item that is created.</response>
        /// <response code = "400">Returns BadRequest with validation error message(s).</response>
        /// <response code = "404">Returns NotFound with message: To-do item cannot be found.</response>
        /// <response code = "500">Returns InternalServerError with message: Something went wrong while creating a to-do item for {currentUser.UserName}. Please try again in a while.</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ToDoItem toDoItem)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (currentUser == null)
                return Unauthorized();

            try
            {
                var result = await this._todoService.CreateItem(currentUser, toDoItem);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    $"Something went wrong while creating a to-do item for {currentUser.UserName}. Please try again in a while.");
            }
        }

        /// <summary>
        /// Updates to-do item for current user.
        /// </summary>
        /// <param name="toDoItem"></param>
        /// <response code = "200">Returns an updated to-do item.</response>
        /// <response code = "400">Returns BadRequest with validation error message(s).</response>
        /// <response code = "404">Returns NotFound with message: T-do item cannot be found.</response>
        /// <response code = "500">Returns InternalServerError with message: Something went wrong while updating a To-Do item for {currentUser.UserName}. Please try again in a while.</response>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ToDoItem toDoItem)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (currentUser == null)
                return Unauthorized();

            try
            {
                var result = await this._todoService.UpdateItem(currentUser, toDoItem);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    $"Something went wrong while updating a To-Do item for {currentUser.UserName}. Please try again in a while.");
            }
        }

        /// <summary>
        /// Deletes to-do item for current user.
        /// </summary>
        /// <param name="itemId"></param>
        /// <response code = "200">Returns Ok.</response>
        /// <response code = "400">Returns BadRequest with validation error message(s).</response>
        /// <response code = "404">Returns NotFound with message: To-do item cannot be found.</response>
        /// <response code = "500">Returns InternalServerError with message: Something went wrong while deleting a To-Do item for {currentUser.UserName}. Please try again in a while.</response>
        [HttpDelete]
        public async Task<IActionResult> Delete([Required] int itemId)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (currentUser == null)
                return Unauthorized();

            try
            {
                await this._todoService.DeleteItem(currentUser, itemId);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    $"Something went wrong while deleting a To-Do item for {currentUser.UserName}. Please try again in a while.");
            }
        }

        /// <summary>
        /// Patches to-do item status.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemStatus"></param>
        /// <response code = "200">Returns a patched to-do item.</response>
        /// <response code = "400">Returns BadRequest with validation error message(s).</response>
        /// <response code = "404">Returns NotFound with message: To-do item cannot be found.</response>
        /// <response code = "500">Returns InternalServerError with message: Something went wrong while updating status of a To-Do item for {currentUser.UserName} to {itemStatus}. Please try again in a while.</response>
        [HttpPatch("{itemId:int}/status/{itemStatus:itemStatus}")]
        public async Task<IActionResult> PatchStatus([Required] int itemId, [Required] ToDoStatuses itemStatus)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (currentUser == null)
                return Unauthorized();

            try
            {
                var result =
                    await this._todoService.PatchItemStatus(currentUser, itemId, itemStatus);
                return Ok(result);
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    $"Something went wrong while updating status of a To-Do item for {currentUser.UserName} to {itemStatus}. Please try again in a while.");
            }
        }
    }
}