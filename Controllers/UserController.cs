using FivD.dbContextClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FivD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : Controller
    {

        private readonly UserDbContext _context;

        public UserController(UserDbContext context)
        {
            _context = context;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.UserEntity
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.PhoneNumber,
                    u.Gender,
                    RegisteredOn = u.RegisteredOn.ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdatedOn = u.UpdatedOn.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.UserEntity.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            _context.UserEntity.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully" });
        }


        [HttpPut("update-user-details/{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDetails userDto)
        {
            var user = await _context.UserEntity.FindAsync(id);
            if (user == null)
                return NotFound();

            // Mapping updated fields from userDto to user entity
            user.Name = userDto.Name;
            user.Email = userDto.Email;
            user.PhoneNumber = userDto.PhoneNumber;
            user.Gender = userDto.Gender;
            user.UpdatedOn = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "User updated successfully", user });
        }


        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteMultipleUsers([FromBody] List<int> userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                return BadRequest(new { message = "No user IDs provided" });
            }

            var users = await _context.UserEntity
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();

            if (!users.Any())
            {
                return NotFound(new { message = "No matching users found" });
            }

            _context.UserEntity.RemoveRange(users);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"{users.Count} user(s) deleted successfully" });
        }

    }
}
