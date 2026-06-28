using Microsoft.AspNetCore.Mvc;

namespace T4.Controllers;

[ApiController]
[Route("[controller]")]
public class OpenController : ControllerBase
{
    [HttpGet("Echo1")]
    public ActionResult<string> Echo1([FromQuery] string name)
    {
        return Content($"Hello {name}");
    }
}
