using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Backend.Authentication;
using Backend.Database;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Backend.Controllers
{
    public class AuthController : BaseController
    {
        private readonly DefaultDbContext _dbContext;
        private readonly IDistributedCache _cache;

        public AuthController(DefaultDbContext dbContext, IDistributedCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
        {
            if (model == null)
                return BadRequest("Invalid model.");

            var user = await _dbContext.Users
                                .Where(a => a.Email.Equals(model.Email))
                                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("Cannot find an user with this e-mail.");

            var token = Guid.NewGuid().ToString();

            var userBytes = JsonSerializer.SerializeToUtf8Bytes(user);

            await _cache.SetAsync(token, userBytes, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
            });

            return Ok(token);
        }

        [HttpDelete]
        public async Task<IActionResult> LogoutAsync()
        {
            var token = Request.Headers[DefaultAuthScheme.SCHEME_NAME];

            await _cache.RemoveAsync(token);

            return Ok();
        }
    }
}