using Microsoft.AspNetCore.Mvc;

namespace LE.FileService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        [HttpGet("status")]
        public IActionResult Status() => Ok();
    }
}
