using System.Threading.Tasks;
using Backend.Database;
using Backend.Database.Models;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [AllowAnonymous]
    public class RegisterController : BaseController
    {
        private readonly DefaultDbContext _dbContext;

        public RegisterController(DefaultDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (model == null)
                return BadRequest("Invalid model");

            if (await _dbContext.Users.AnyAsync(a => a.Email.Equals(model.Email)))
                return UnprocessableEntity("Already exists an user with this e-mail.");

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Password = model.Password,
            };

            await _dbContext.Users.AddAsync(user);

            await _dbContext.SaveChangesAsync();

            user.Password = null;

            return Created($"{HttpContext.Request.Host}users/{user.Id}", user);
        }
    }
}