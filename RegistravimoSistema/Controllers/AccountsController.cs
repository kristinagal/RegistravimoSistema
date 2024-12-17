using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegistravimoSistema.DTOs;
using RegistravimoSistema.Mappers;
using RegistravimoSistema.Repositories;
using RegistravimoSistema.Services;
using System.ComponentModel.DataAnnotations;

namespace RegistravimoSistema.Controllers
{
    /// <summary>
    /// Vartotojų registracija, prisijungimas ir trynimas.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IAccountService _accountService;
        private readonly IAccountMapper _accountMapper;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(
            IUserRepository userRepository,
            IJwtService jwtService,
            IAccountService accountService,
            IAccountMapper accountMapper,
            ILogger<AccountsController> logger)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _accountService = accountService;
            _accountMapper = accountMapper;
            _logger = logger;
        }

        /// <summary>
        /// Naujo vartotojo registracija.
        /// </summary>
        /// <param name="request">Vartotojo registracijos duomenys.</param>
        /// <returns>201 Created, jei registracija sėkminga.</returns>
        [HttpPost("SignUp")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignUp([FromBody, Required] UserAuthRequest request)
        {
            _logger.LogInformation("Pradedama naujo vartotojo registracija su vardu: {Username}", request.Username);

            var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                _logger.LogWarning("Registracijos klaida: vartotojo vardas '{Username}' jau egzistuoja.", request.Username);
                return BadRequest("Username already exists.");
            }

            _accountService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = _accountMapper.MapFromDto(request);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Role = "User"; // default

            await _userRepository.AddAsync(user);

            _logger.LogInformation("Vartotojas '{Username}' sėkmingai užregistruotas.", request.Username);
            return Created("", new { message = "User registered successfully!" });
        }

        /// <summary>
        /// Vartotojo prisijungimas.
        /// </summary>
        /// <param name="request">Vartotojo prisijungimo duomenys.</param>
        /// <returns>200 OK su JWT, jei prisijungimas sėkmingas.</returns>
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody, Required] UserAuthRequest request)
        {
            _logger.LogInformation("Vartotojo '{Username}' prisijungimo procesas pradėtas.", request.Username);

            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null)
            {
                _logger.LogWarning("Prisijungimo klaida: vartotojas '{Username}' nerastas.", request.Username);
                return Unauthorized("User not found.");
            }

            if (!_accountService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                _logger.LogWarning("Prisijungimo klaida: neteisingas slaptažodis vartotojui '{Username}'.", request.Username);
                return Unauthorized("Incorrect password.");
            }

            var token = _jwtService.GenerateJwtToken(user);
            _logger.LogInformation("Vartotojas '{Username}' sėkmingai prisijungė.", request.Username);
            return Ok(new { Token = token });
        }

        /// <summary>
        /// Ištrina vartotoją ir susijusius duomenis (tik administratoriams).
        /// </summary>
        /// <param name="id">Ištrinamo vartotojo unikalus ID.</param>
        /// <returns>204 NoContent, jei ištrynimas sėkmingas.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteUser/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser([FromRoute, Required] Guid id)
        {
            _logger.LogInformation("Vartotojo su ID {UserId} šalinimo procesas pradėtas.", id);

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Šalinimo klaida: vartotojas su ID {UserId} nerastas.", id);
                return NotFound("User not found.");
            }

            try
            {
                await _accountService.DeleteUserWithPersonAsync(id);
                _logger.LogInformation("Vartotojas su ID {UserId} sėkmingai ištrintas.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Klaida bandant ištrinti vartotoją su ID {UserId}.", id);
                return StatusCode(500, $"An error occurred while deleting the user: {ex.Message}");
            }
        }
    }
}
