using System.Threading.Tasks;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
        {
            await Task.Delay(0);

            return Ok();
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> LogoutAsync()
        {
            await Task.Delay(0);

            return Ok();
        }
    }
}