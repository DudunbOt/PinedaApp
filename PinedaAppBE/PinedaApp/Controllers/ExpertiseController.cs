using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models.Errors;
using PinedaApp.Services;

namespace PinedaApp.Controllers
{
    public class ExpertiseController : BaseApiController
    {
        private readonly IExpertiseService _expertiseService;
        private int newId = 0;
        public ExpertiseController(IExpertiseService expertiseService)
        {
            _expertiseService = expertiseService;
        }

        [HttpGet]
        public IActionResult GetExpertises()
        {
            try
            {
                Response responses = _expertiseService.GetExpertises();
                return Ok(responses);
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse response = new(ex.Message);
                return StatusCode(ex.ErrorCode, response);
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetExpertise(int id)
        {
            try
            {
                Response response = _expertiseService.GetExpertise(id);
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
        public IActionResult CreateExpertise([FromForm] ExpertiseRequest request)
        {
            try
            {
                Response response = _expertiseService.UpsertExpertise(request, out newId);

                return CreatedAtAction
                (
                    actionName: nameof(GetExpertise),
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

        [Authorize]
        [HttpPut("{id:int}")]
        public IActionResult UpdateExpertise(int id, [FromForm] ExpertiseRequest request)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0 || !CheckUserOwner(id, userId))
                {
                    ErrorResponse forbidden = new("Not Allowed to Update Data");
                    return StatusCode(403, forbidden);
                }
                Response updatedExpertise = _expertiseService.UpsertExpertise(request, out newId, id);

                if (newId == id) return NoContent();

                return CreatedAtAction
                (
                    actionName: nameof(GetExpertise),
                    routeValues: new { id = newId },
                    value: updatedExpertise
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
        public IActionResult DeleteExpertise(int id)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0 || !CheckUserOwner(id, userId))
                {
                    ErrorResponse forbidden = new("Not Allowed to Delete Data");
                    return StatusCode(403, forbidden);
                }

                _expertiseService.DeleteExpertise(id);
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
