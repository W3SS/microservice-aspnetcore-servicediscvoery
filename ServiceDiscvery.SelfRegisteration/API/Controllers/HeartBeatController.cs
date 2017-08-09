using Microsoft.AspNetCore.Mvc;

namespace ServiceDiscvery.SelfRegisteration.Controllers
{
    [Route("api/[controller]")]
    public class HeartBeatController : Controller
    {
        [HttpGet("status")]        
        public IActionResult Status() => Ok();
    }
}
