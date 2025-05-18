using Microsoft.AspNetCore.Mvc;
using FivD.Models;
using FivD.dbContextClass;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FivD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config, UserDbContext context)
        {
            _config = config;
            _context = context;
        }


        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLogin request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.UserEntity.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return Unauthorized("Invalid email or password");

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                return Unauthorized("Invalid email or password");

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }



        private string GenerateJwtToken(UserEntity user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("FullName", user.Name),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_config["JWT:TokenDurationInMinutes"]));

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

 

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegister userRegister)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Checking if user already exists
            if (_context.UserEntity.Any(u => u.Email == userRegister.Email))
                return BadRequest("Email already in use.");

            CreatePasswordHash(userRegister.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new UserEntity
            {
                Email = userRegister.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Name = userRegister.Name,
                PhoneNumber = userRegister.PhoneNumber,
                Gender = userRegister.Gender,
                RegisteredOn = DateTime.UtcNow
            };

            _context.UserEntity.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully." });
        }

        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using (var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
