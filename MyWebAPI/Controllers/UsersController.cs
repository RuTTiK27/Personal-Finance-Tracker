using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;

namespace MyWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController:ControllerBase
    {
        private readonly MyAppDbContext _context;

        public UsersController(MyAppDbContext context)
        {
            _context = context;
        }

        // GET: api/users/reservedusernames
        [HttpGet("reservedusernames")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<ActionResult<IEnumerable<string>>> GetReservedUsernames()
        {
            // Fetch only the 'Username' column from the 'Users' table
            var reservedUsernames = await _context.Users
                                                  .Select(u => u.Username)
                                                  .ToListAsync();

            return Ok(reservedUsernames);
        }
        //GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        //GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        //POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            user.CreatedDate = DateTime.Now;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new {id = user.UserId},user);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        //PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id,[FromBody] UserUpdateDto userUpdateDto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Update fields only if they are provided
            if (!string.IsNullOrEmpty(userUpdateDto.Username))
            {
                user.Username = userUpdateDto.Username;
            }
            if (!string.IsNullOrEmpty(userUpdateDto.Email))
            {
                user.Email = userUpdateDto.Email;
            }
            if (!string.IsNullOrEmpty(userUpdateDto.PasswordHash))
            {
                user.PasswordHash = userUpdateDto.PasswordHash;
            }
            
            _context.Entry(user).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        //DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}