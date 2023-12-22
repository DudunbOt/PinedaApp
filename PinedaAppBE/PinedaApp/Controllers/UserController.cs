﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models.Errors;
using PinedaApp.Services;
using System.Security.Claims;

namespace PinedaApp.Controllers
{
    public class UserController : BaseApiController
    {
        private readonly IUserService userService;
        private int newId = 0;
        public UserController(IUserService _userService)
        {
            userService = _userService;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
                Response responses = userService.GetUsers();
                return Ok(responses);
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse response = new(ex.Message);
                return StatusCode(ex.ErrorCode, response);
            }
        }

        [HttpPost("GetToken")]
        public IActionResult GetToken(LoginRequest request)
        {
            try
            {
                Response token = userService.GetToken(request.UserName, request.Password);
                return Ok(token);
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse response = new(ex.Message);
                return StatusCode(ex.ErrorCode, response);
            }
        }

        
        [HttpPost]
        public IActionResult CreateUser([FromForm] UserRequest request)
        {
            try
            {
                Response response = userService.UpsertUser(request, out newId);

                return CreatedAtAction
                (
                    actionName: nameof(GetUser),
                    routeValues: new { id = newId },
                    value: response
                );
            }
            catch (PinedaAppException ex)
            {
                if (ex.InnerException is ValidationException validationEx)
                {
                    ErrorResponse response = new(validationEx.ValidationErrors.Errors);
                    return BadRequest(response);
                }
                else
                {
                    ErrorResponse response = new(ex.Message);
                    return StatusCode(ex.ErrorCode, response);
                }
            }

        }

        [HttpGet("{id:int}")]
        public IActionResult GetUser(int id)
        {
            try
            {
                Response user = userService.GetUser(id);

                return Ok(user);
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse response = new(ex.Message);
                return NotFound(response);
            }

        }

        [Authorize]
        [HttpPut("{id:int}")]
        public IActionResult UpdateUser(int id, [FromForm] UserRequest request)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0 || !CheckUserOwner(id, userId))
                {
                    ErrorResponse forbidden = new("Not Allowed to Update Data");
                    return StatusCode(403, forbidden);
                }

                Response updatedUser = userService.UpsertUser(request, out newId, id);

                if (newId == id) return NoContent();

                return CreatedAtAction
                (
                    actionName: nameof(GetUser),
                    routeValues: new { id = newId },
                    value: updatedUser
                );
            }
            catch (PinedaAppException ex)
            {
                if (ex.InnerException is ValidationException validationEx)
                {
                    ErrorResponse response = new(validationEx.ValidationErrors.Errors);
                    return BadRequest(response);
                }
                else
                {
                    ErrorResponse response = new(ex.Message);
                    return StatusCode(ex.ErrorCode, response);
                }
            }
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0 || !CheckUserOwner(id, userId))
                {
                    ErrorResponse forbidden = new("Not Allowed to Delete Data");
                    return StatusCode(403, forbidden);
                }

                userService.DeleteUser(id);
                return NoContent();
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse response = new(ex.Message);
                return StatusCode(ex.ErrorCode, response);
            }

        }

    }
}