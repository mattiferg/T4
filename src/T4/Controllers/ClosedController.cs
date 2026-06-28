using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace T4.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ClosedController : ControllerBase
{
    [HttpGet("Echo2")]
    public ActionResult<string> Echo2([FromQuery] string name)
    {
        return Content($"Good to see you again {name}");
    }
}
