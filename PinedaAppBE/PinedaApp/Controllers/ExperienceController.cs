using Microsoft.AspNetCore.Mvc;
using PinedaApp.Configurations;
using PinedaApp.Contracts;
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
                ErrorResponse error = new ErrorResponse(ex.Message);
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
                ErrorResponse error = new ErrorResponse(ex.Message);
                return StatusCode(ex.ErrorCode, error);
            }
        }

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

        [HttpPut("{id:int}")]
        public IActionResult UpdateExperience(int id, ExperienceRequest request)
        {
            try
            {
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

        [HttpDelete("{id:int}")]
        public IActionResult DeleteExperience(int id)
        {
            try
            {
                _experience.DeleteExperience(id);
                return NoContent();
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message);
                return StatusCode(ex.ErrorCode, error);
            }
        }
    }
}