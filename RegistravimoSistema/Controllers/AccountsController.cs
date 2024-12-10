using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RegistravimoSistema.Entities;
using RegistravimoSistema.DTOs;

namespace RegistravimoSistema.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountsController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("SignUp")]
        public IActionResult SignUp([FromBody] UserSignUpRequest request)
        {
            if (_context.Users.Any(u => u.Username == request.Username))
                return BadRequest("Username already exists.");

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = "User" // default
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Created("", new { message = "User registered successfully!" });
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] UserLoginRequest request)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == request.Username);
            if (user == null)
                return Unauthorized("User not found.");

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                return Unauthorized("Incorrect password.");

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteUser/{id:guid}")]
        public IActionResult DeleteUser(Guid id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound("User not found.");

            _context.Users.Remove(user);
            _context.SaveChanges();
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users
                .Select(u => new { u.Id, u.Username, u.Role })
                .ToList();

            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetUser/{id:guid}")]
        public IActionResult GetUser(Guid id)
        {
            var user = _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Role,
                    Persons = u.Persons.Select(p => new
                    {
                        p.Vardas,
                        p.Pavarde,
                        p.AsmensKodas,
                        p.TelefonoNumeris,
                        p.ElPastas,
                        p.Address
                    })
                })
                .SingleOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound("User not found.");

            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("UpdateUserRole/{id:guid}")]
        public IActionResult UpdateUserRole(Guid id, [FromBody] UpdateUserRoleRequest request)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound("User not found.");

            if (request.Role != "Admin" && request.Role != "User")
                return BadRequest("Invalid role. Allowed values are 'Admin' or 'User'.");

            user.Role = request.Role;
            _context.SaveChanges();

            return Ok(new { message = "User role updated successfully!" });
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class UpdateUserRoleRequest
    {
        public string Role { get; set; }
    }
}
