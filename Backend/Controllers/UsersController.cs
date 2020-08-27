using System.Linq;
using System.Threading.Tasks;
using Backend.Database;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    public class UsersController : BaseController
    {
        private readonly DefaultDbContext _dbContext;

        public UsersController(DefaultDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var users = await _dbContext.Users.ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid id.");

            var user = await _dbContext.Users
                                .Where(a => a.Id == id)
                                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("Cannot find an user with this id.");

            return Ok(user);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] RegisterModel model)
            => RedirectToAction(nameof(RegisterController.RegisterAsync), nameof(RegisterController));

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] RegisterModel model)
        {
            if (model == null)
                return BadRequest("Invalid model");

            var user = await _dbContext.Users
                                .Where(a => a.Email.Equals(model.Email))
                                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("Cannot find an user with this e-mail.");

            user.Name = model.Name;
            user.Password = model.Password;

            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid id.");

            var user = await _dbContext.Users
                                .Where(a => a.Id == id)
                                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("Cannot find an user with this id.");

            _dbContext.Users.Remove(user);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}