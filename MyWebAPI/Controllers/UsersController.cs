using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace MyWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MyAppDbContext _context;

        private readonly IConfiguration _configuration;
        public UsersController(MyAppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

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

        // GET: api/users/reservedemails
        [HttpGet("reservedemails")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<ActionResult<IEnumerable<string>>> GetReservedEmails()
        {
            // Fetch only the 'Email' column from the 'Users' table
            var reservedEmails = await _context.Users
                                                  .Select(u => u.Email)
                                                  .ToListAsync();

            return Ok(reservedEmails);
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
        [EnableCors("AllowSpecificOrigin")]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            user.CreatedDate = DateTime.Now;
            user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);
            // Generate JWT Token for verification
            var EmailVerificationToken = GenerateJwtToken(user);
            user.EmailVerificationToken = EmailVerificationToken.ToString();

            // Send verification email
            await SendVerificationEmail(user.Email, EmailVerificationToken.ToString());

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        //PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto userUpdateDto)
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
            catch (DbUpdateConcurrencyException)
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

        [HttpPost("generateToken")]
        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // Adding UserId as a claim
                new Claim(ClaimTypes.Email, user.Email) // Adding email as a claim
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationInMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpPost("sendVerificationEmail")]
        public async Task<ActionResult> SendVerificationEmail(string recipientEmail, string token)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential("xeroxshops2021@gmail.com", "yhqz tkdw eumm xukk"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("xeroxshops2021@gmail.com"),
                Subject = "Verify Your Email",
                Body = $"Please verify your email by clicking this link: http://localhost:5021/api/users/verify-email?token={token}",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(recipientEmail);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                return Ok("Verification email sent successfully.");
            }
            catch (Exception ex)
            {
                // Log exception (not shown here)
                return StatusCode(StatusCodes.Status500InternalServerError, "Error sending email.");
            }
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            // Decode the token to get the email claim
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var email = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (email == null)
            {
                return BadRequest("Invalid token.");
            }

            // Find the user by email
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // Update user verification status
            user.IsEmailConfirmed = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Email verified successfully. Visit http://localhost:4200/login");
        }

        [HttpPost("validate-user")]
        public async Task<IActionResult> ValidateUser([FromBody] User user)
        {

            // Find the user by email
            var userFound = await _context.Users.SingleOrDefaultAsync(u => u.Email == user.Email || u.Username == user.Username && user.PasswordHash == user.PasswordHash);

            if (userFound == null)
            {
                return BadRequest("User not found with this credentials.");
            }



            return Ok("Email verified successfully. Visit http://localhost:4200/login");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == request.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid email" });

            // Verify password
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (passwordVerificationResult != PasswordVerificationResult.Success)
                return Unauthorized(new { message = "Invalid password" });

            // Check if email is confirmed
            if (!user.IsEmailConfirmed)
                return BadRequest(new { message = "Email not verified" });

            // Generate JWT token using the existing method
            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            // Save Refresh Token and its expiry
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(2); // Refresh token expiry (2 days)

            _context.SaveChanges();

            return Ok(new AuthResponse
            {
                accessToken = accessToken,
                refreshToken = refreshToken
            });
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] AuthResponse authResponse)
        {
            var user = _context.Users.SingleOrDefault(u => u.RefreshToken == authResponse.refreshToken);
            if (user == null)
                return BadRequest();   
            if(user.RefreshTokenExpiryTime < DateTime.Now)
                return BadRequest();

            var newJwtToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // Save Refresh Token and its expiry
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(2); // Refresh token expiry (2 days)

            _context.SaveChanges();

            return Ok(new
            {
                accessToken = newJwtToken,
                refreshToken = newRefreshToken
            });
        }
    }
}