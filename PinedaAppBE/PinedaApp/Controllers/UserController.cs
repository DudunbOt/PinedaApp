using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models.Errors;
using PinedaApp.Services;

namespace PinedaApp.Controllers
{
    public class UserController : BaseApiController
    {
        private readonly IUserService userService;
        public UserController(IUserService _userService)
        {
            userService = _userService;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
                List<UserResponse> responses = userService.GetUsers();
                return Ok(responses);
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return StatusCode(ex.ErrorCode, response);
            }
        }

        [HttpPost("GetToken")]
        public IActionResult GetToken(LoginRequest request)
        {
            try
            {
                LoginResponse token = userService.GetToken(request.UserName, request.Password);
                return Ok(token);
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return StatusCode(ex.ErrorCode, response);
            }
        }

        [HttpPost]
        public IActionResult CreateUser(UserRequest request)
        {
            try
            {
                UserResponse response = userService.UpsertUser(request);

                return CreatedAtAction
                (
                    actionName: nameof(GetUser),
                    routeValues: new { id = response.Id },
                    value: response
                );
            }
            catch (PinedaAppException ex)
            {
                if (ex.InnerException is ValidationException validationEx)
                {
                    ErrorResponse response = new ErrorResponse(validationEx.ValidationErrors.Errors);
                    return BadRequest(response);
                }
                else
                {
                    ErrorResponse response = new ErrorResponse(ex.Message);
                    return StatusCode(ex.ErrorCode, response);
                }
            }

        }

        [HttpGet("{id:int}")]
        public IActionResult GetUser(int id)
        {
            try
            {
                UserResponse user = userService.GetUser(id);

                return Ok(user);
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return NotFound(response);
            }

        }

        [Authorize]
        [HttpPut("{id:int}")]
        public IActionResult UpdateUser(int id, UserRequest request)
        {
            try
            {
                UserResponse updatedUser = userService.UpsertUser(request, id);

                if (updatedUser.Id == id) return NoContent();

                return CreatedAtAction
                (
                    actionName: nameof(GetUser),
                    routeValues: new { id = updatedUser.Id },
                    value: updatedUser
                );
            }
            catch (PinedaAppException ex)
            {
                if (ex.InnerException is ValidationException validationEx)
                {
                    ErrorResponse response = new ErrorResponse(validationEx.ValidationErrors.Errors);
                    return BadRequest(response);
                }
                else
                {
                    ErrorResponse response = new ErrorResponse(ex.Message);
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
                userService.DeleteUser(id);
                return NoContent();
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return StatusCode(ex.ErrorCode, response);
            }

        }


    }
}