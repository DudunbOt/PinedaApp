using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace PinedaApp.Configurations
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BaseApiController : ControllerBase
    { 

    }
}
