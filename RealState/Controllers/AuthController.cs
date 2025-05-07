using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using RealState.BAL.DTO;
using RealState.Data;
using RealState.Services;
using System.Data.Entity;

namespace RealState.Controllers
{
   
    public class AuthController : Controller
    {
        private readonly TokenService _tokenService;
        private readonly ApplicationDbContext _context;
        public AuthController(TokenService tokenService, ApplicationDbContext context)
        {
            _tokenService = tokenService;
            _context = context;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            // Replace with password hashing in production
                var user = _context.Users
                .FirstOrDefault(u => u.UserName == request.Email && u.Password == request.Password);


            if (user == null)
                return Unauthorized("Invalid username or password");

            var token = _tokenService.GenerateToken(user.UserName, "Admin");
            return Ok(new { Token = token });
        }

        [HttpGet("getUser")]
        [Authorize] // Ensure that the user is authenticated to access this endpoint
        public IActionResult GetUser()
        {
            // The user's identity will be accessible through User.Identity
            var username = User.Identity.Name;  // Extract the username from the JWT claims

            var user = _context.Users.FirstOrDefault(u => u.UserName == username);

            if (user == null)
                return Unauthorized("User not found");

            return Ok(new { User = user });
        }
    }
}
