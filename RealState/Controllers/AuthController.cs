using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using RealState.BAL.DTO;
using RealState.BAL.Helpers;
using RealState.BAL.ILogic;
using RealState.Data;
using RealState.Services;
using System.Data.Entity;

namespace RealState.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // This matters
    public class AuthController : Controller
    {
        private readonly TokenService _tokenService;
        private readonly ApplicationDbContext _context;

        protected IContactLogic _contactLogic { get; private set; }
        public AuthController(TokenService tokenService, ApplicationDbContext context, IContactLogic contactLogic)
        {
            _tokenService = tokenService;
            _context = context;
            _contactLogic = contactLogic;
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
            return Ok(new { Token = token ,UserName = user.UserName });
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


        [HttpPost("updateContactResponse")]
        public IActionResult UpdateContactResponse(int id, string? responseMessage)
        {
            var contact = _context.Contact.FirstOrDefault(c => c.Id == id);
            if (contact == null)
                return NotFound($"No contact found with ID {id}");

            contact.ResponseStatus = true;
            contact.ResponseDate = DateTime.UtcNow;
            contact.ResponseMessage = responseMessage;
            _context.SaveChanges();
            return Ok(new { message = "Response updated successfully", contact });
        }


        // [Authorize]
        [HttpGet("getContactList")]
        public IActionResult GetContactList()
        {
            var contactList = _context.Contact
         .OrderByDescending(c => c.CreatedDate)
         .ToList();

            if (contactList == null || !contactList.Any())
                return NotFound("No contacts found");

            return Ok(contactList); // ✅ Just return the list directly
        }

        [HttpGet]
        [Route("GetContactStatData")]
        public IActionResult GetContactStatData()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var responseObj = _contactLogic.GetContactStats("");
                if (responseObj == null)
                    return NotFound();
                return Ok(responseObj);
            }
            catch (AppException ex)
            {


                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
