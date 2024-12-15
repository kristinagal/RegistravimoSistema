using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistravimoSistema.DTOs;
using RegistravimoSistema.Mappers;
using RegistravimoSistema.Repositories;
using RegistravimoSistema.Services;

namespace RegistravimoSistema.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IAccountService _accountService;
        private readonly IAccountMapper _accountMapper;

        public AccountsController(
            IUserRepository userRepository,
            IJwtService jwtService,
            IAccountService accountService,
            IAccountMapper accountMapper)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _accountService = accountService;
            _accountMapper = accountMapper;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] UserAuthRequest request)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUser != null)
                return BadRequest("Username already exists.");

            _accountService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = _accountMapper.MapFromDto(request);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Role = "User"; // default

            await _userRepository.AddAsync(user);

            return Created("", new { message = "User registered successfully!" });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserAuthRequest request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null)
                return Unauthorized("User not found.");

            if (!_accountService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                return Unauthorized("Incorrect password.");

            var token = _jwtService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteUser/{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound("User not found.");

            await _userRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
