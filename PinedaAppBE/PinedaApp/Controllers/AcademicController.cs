using Microsoft.AspNetCore.Authorization;
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
        private int newId;
        public AcademicController(IAcademicServices academicService)
        {
            _academicService = academicService;
        }

        [HttpGet]
        public IActionResult GetAcademics()
        {
            try
            {
                Response responses = _academicService.GetAcademics();
                return Ok(responses);
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse response = new(ex.Message);
                return StatusCode(ex.ErrorCode, response);
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetAcademic(int id)
        {
            try
            {
                Response response = _academicService.GetAcademic(id);
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
        public IActionResult CreateAcademic([FromForm] AcademicRequest request)
        {
            try
            {
                Response response = _academicService.UpsertAcademic(request, out newId);

                return CreatedAtAction
                (
                    actionName: nameof(GetAcademic),
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
        public IActionResult UpdateAcademic(int id, [FromForm] AcademicRequest request)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0 || !CheckUserOwner(id, userId))
                {
                    ErrorResponse forbidden = new("Not Allowed to Update Data");
                    return StatusCode(403, forbidden);
                }
                Response updatedAcademic = _academicService.UpsertAcademic(request, out newId, id);

                if (newId == id) return NoContent();

                return CreatedAtAction
                (
                    actionName: nameof(GetAcademic),
                    routeValues: new { id = newId },
                    value: updatedAcademic
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
        public IActionResult DeleteAcademic(int id)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0 || !CheckUserOwner(id, userId))
                {
                    ErrorResponse forbidden = new("Not Allowed to Delete Data");
                    return StatusCode(403, forbidden);
                }

                _academicService.DeleteAcademic(id);
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