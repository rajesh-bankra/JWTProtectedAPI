using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTProtectedAPI.Controllers
{
    

    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        [HttpPost("process-input")]
        public IActionResult ProcessInput([FromBody] InputModel input)
        {


            // Your sanitized input is now available in the 'input' object
            return Ok(new { message = "Input processed successfully.:" + Convert.ToInt32(input.Data) });
        }
    }

    public class InputModel
    {
        public string Data { get; set; }
    }

}
