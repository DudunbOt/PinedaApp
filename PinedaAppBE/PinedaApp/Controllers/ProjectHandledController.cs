using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models.Errors;
using PinedaApp.Services;

namespace PinedaApp.Controllers
{
    public class ProjectHandledController : BaseApiController
    {
        private readonly IProjectHandledService _projectHandledService;
        public ProjectHandledController(IProjectHandledService projectHandledService)
        {
            _projectHandledService = projectHandledService;
        }

        [HttpGet]
        public IActionResult GetProjectHandled()
        {
            try
            {
                List<ProjectHandledResponse> responses = _projectHandledService.GetProjectHandled();
                return Ok(responses);
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse response = new(ex.Message);
                return StatusCode(ex.ErrorCode, response);
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetProjectHandled(int id)
        {
            try
            {
                ProjectHandledResponse response = _projectHandledService.GetProjecthandled(id);
                return Ok(response);
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse response = new(ex.Message);
                return StatusCode(ex.ErrorCode, response);
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateProjectHandled([FromForm] ProjectHandledRequest request)
        {
            try
            {
                ProjectHandledResponse response = _projectHandledService.UpsertProjectHandled(request);

                return CreatedAtAction
                (
                    actionName: nameof(GetProjectHandled),
                    routeValues: new { id = response.Id },
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

        [Authorize]
        [HttpPut("{id:int}")]
        public IActionResult UpdateProjectHandled(int id, [FromForm] ProjectHandledRequest request)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0 || !CheckUserOwner(id, userId))
                {
                    ErrorResponse forbidden = new("Not Allowed to Update Data");
                    return StatusCode(403, forbidden);
                }
                ProjectHandledResponse updateProjectHandled = _projectHandledService.UpsertProjectHandled(request, id);

                if (updateProjectHandled.Id == id) return NoContent();

                return CreatedAtAction
                (
                    actionName: nameof(GetProjectHandled),
                    routeValues: new { id = updateProjectHandled.Id },
                    value: updateProjectHandled
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
        public IActionResult DeleteProjectHandled(int id)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0 || !CheckUserOwner(id, userId))
                {
                    ErrorResponse forbidden = new("Not Allowed to Delete Data");
                    return StatusCode(403, forbidden);
                }

                _projectHandledService.DeleteProjectHandled(id);
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
