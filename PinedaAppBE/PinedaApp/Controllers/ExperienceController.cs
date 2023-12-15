using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models;
using PinedaApp.Models.Errors;
using PinedaApp.Services;

namespace PinedaApp.Controllers
{
    public class ExperienceController : BaseApiController
    {
        private readonly IExperienceServices _experience;
        public ExperienceController(IExperienceServices experience)
        {
            _experience = experience;
        }

        [HttpGet]
        public IActionResult GetExperiences()
        {
            try
            {
                List<ExperienceResponse> responses = _experience.GetExperiences();
                return Ok(responses);
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse error = new(ex.Message);
                return StatusCode(ex.ErrorCode, error);
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetExperience(int id)
        {
            try
            {
                ExperienceResponse response = _experience.GetExperience(id);
                return Ok(response);
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse error = new(ex.Message);
                return StatusCode(ex.ErrorCode, error);
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateExperience(ExperienceRequest request)
        {
            try
            {
                ExperienceResponse response = _experience.UpsertExperience(request);
                return CreatedAtAction
                (
                    actionName: nameof(GetExperience),
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
        public IActionResult UpdateExperience(int id, ExperienceRequest request)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0 || !CheckUserOwner(id, userId))
                {
                    ErrorResponse forbidden = new("Not Allowed to Update Data");
                    return StatusCode(403, forbidden);
                }

                ExperienceResponse response = _experience.UpsertExperience(request, id);
                if (response.Id == id) return NoContent();

                return CreatedAtAction
                (
                    actionName: nameof(GetExperience),
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
        [HttpDelete("{id:int}")]
        public IActionResult DeleteExperience(int id)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0 || !CheckUserOwner(id, userId))
                {
                    ErrorResponse forbidden = new("Not Allowed to Delete Data");
                    return StatusCode(403, forbidden);
                }

                _experience.DeleteExperience(id);
                return NoContent();
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse error = new(ex.Message);
                return StatusCode(ex.ErrorCode, error);
            }
        }
    }
}