using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PinedaApp.Configurations
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1.0/[controller]")]
    public class BaseApiController<TService>(TService service) : ControllerBase where TService : class
    {
        protected readonly TService _service = service;
        private int _userId;
        protected int newId;
        protected int GetUserId()
        {
            if (_userId != 0) return _userId;

            return Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        protected bool CheckUserOwner(int id, int userId)
        {
            string? userRole = HttpContext.User.FindFirstValue(ClaimTypes.Role);
            if (userRole == "Owner") return true;
            if (userId == 0) return false;
            if (id == 0) return false;
            if (id != userId) return false;

            return true;
        }
    }
}
