using Microsoft.AspNetCore.Mvc;
using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models.Errors;
using PinedaApp.Services;

namespace PinedaApp.Controllers
{
    public class AcademicController : BaseApiController
    {
        private readonly IAcademicServices _academicService;
        public AcademicController(IAcademicServices academicService)
        {
            _academicService = academicService;
        }

        [HttpGet]
        public IActionResult GetAcademics()
        {
            try
            {
                List<AcademicResponse> responses = _academicService.GetAcademics();
                return Ok(responses);
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return StatusCode(ex.ErrorCode, response);
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetAcademic(int id)
        {
            try
            {
                AcademicResponse response = _academicService.GetAcademic(id);
                return Ok(response);
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return StatusCode(ex.ErrorCode, response);
            }
        }

        [HttpPost]
        public IActionResult CreateAcademic(AcademicRequest request)
        {
            try
            {
                AcademicResponse response = _academicService.UpsertAcademic(request);

                return CreatedAtAction
                (
                    actionName: nameof(GetAcademic),
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
        public IActionResult UpdateAcademic(int id, AcademicRequest request)
        {
            try
            {
                AcademicResponse updatedAcademic = _academicService.UpsertAcademic(request, id);

                if (updatedAcademic.Id == id) return NoContent();

                return CreatedAtAction
                (
                    actionName: nameof(GetAcademic),
                    routeValues: new { id = updatedAcademic.Id },
                    value: updatedAcademic
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
        public IActionResult DeleteAcademic(int id)
        {
            try
            {
                _academicService.DeleteAcademic(id);
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