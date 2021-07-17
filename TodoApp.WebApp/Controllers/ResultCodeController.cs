using Microsoft.AspNetCore.Mvc;

namespace TodoApp.WebApp.Controllers
{
    [ApiController]
    [Route("test/resultcode")]
    public class ResultCodeController : ControllerBase
    {
        [HttpGet("{code:int}")]
        public IActionResult Get([FromRoute] int code)
        {
            return StatusCode(code);
        }
    }
}