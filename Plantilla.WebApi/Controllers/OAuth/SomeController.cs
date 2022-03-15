using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Plantilla.WebApi.Controllers.OAuth
{

    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class SomeController : Controller
    {

        [HttpGet("hello1")]
        [Authorize(Roles = "Role1")]
        public async Task<IActionResult> SayHello1()
        {
            return await Task.FromResult(Ok(User.Claims.Select(c => c.Type + " - " + c.Value).ToList()));
        }

        [HttpGet("hello2")]
        [Authorize(Roles = "Role2")]
        public async Task<IActionResult> SayHello2()
        {
            return await Task.FromResult(Ok(User.Claims.Select(c => c.Type + " - " + c.Value).ToList()));
        }

        [HttpGet("hello3")]
        [Authorize(Policy = "WithTwoRoles")]
        public async Task<IActionResult> SayHello3()
        {
            return await Task.FromResult(Ok(User.Claims.Select(c => c.Type + " - " + c.Value).ToList()));
        }
    }
}
