﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDo.Persistent.DbEnums;
using ToDo.Persistent.DbObjects;
using ToDo.Persistent.DbServices;

namespace ToDo.WebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/todos")]
    [Authorize]
    public class ToDoController : Controller
    {
        private readonly IToDoService _todoService;
        private readonly UserManager<IdentityUser> _userManager;

        public ToDoController(IToDoService todoService, UserManager<IdentityUser> userManager)
        {
            this._todoService = todoService;
            this._userManager = userManager;
        }

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
                    $"Something went wrong while getting all the ToDo items for {currentUser.UserName}. Please try again in a while.");
            }
        }

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
                    $"Something went wrong while getting all the ToDo items for {currentUser.UserName} by status {itemStatus}. Please try again in a while.");
            }
        }

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
                    $"Something went wrong while getting all the ToDo items for {currentUser.UserName} by Id {itemId}. Please try again in a while.");
            }
        }

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
                    $"Something went wrong while creating a ToDo item for {currentUser.UserName}. Please try again in a while.");
            }
        }

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
                    $"Something went wrong while updating a ToDo item for {currentUser.UserName}. Please try again in a while.");
            }
        }

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
                    $"Something went wrong while deleting a ToDo item for {currentUser.UserName}. Please try again in a while.");
            }
        }

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
                    $"Something went wrong while updating status of a ToDo item for {currentUser.UserName} to {itemStatus}. Please try again in a while.");
            }
        }
    }
}