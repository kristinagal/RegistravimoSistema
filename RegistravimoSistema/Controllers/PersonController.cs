using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegistravimoSistema.DTOs;
using RegistravimoSistema.Entities;
using RegistravimoSistema.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RegistravimoSistema.Controllers
{
    /// <summary>
    /// Atsakingas už asmeninių duomenų kūrimą, gavimą ir atnaujinimą.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;
        private readonly ILogger<PersonController> _logger;

        public PersonController(IPersonService personService, ILogger<PersonController> logger)
        {
            _personService = personService;
            _logger = logger;
        }

        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                _logger.LogWarning("Autentifikacijos klaida: vartotojas neprisijungęs.");
                throw new UnauthorizedAccessException("User must be authenticated.");
            }
            return parsedUserId;
        }

        /// <summary>
        /// Sukuria naują asmens įrašą.
        /// </summary>
        /// <param name="request">Asmens sukūrimo užklausa.</param>
        /// <returns>201 Created su sukurtais duomenimis.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePerson([FromBody, Required] PersonRequest request)
        {
            _logger.LogInformation("Pradedamas naujo asmens kūrimo procesas.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Asmens kūrimo užklausa neteisinga.");
                return BadRequest(ModelState);
            }

            try
            {
                var userId = GetCurrentUserId();

                var existingPerson = await _personService.GetPersonByUserIdAsync(userId);
                if (existingPerson != null)
                {
                    _logger.LogWarning("Klaida: vartotojas su ID {UserId} jau turi sukurtą asmenį.", userId);
                    return Conflict("You have already created a person. A user can only create one person.");
                }

                await _personService.CreatePersonAsync(request, userId);
                _logger.LogInformation("Asmens įrašas sėkmingai sukurtas vartotojui {UserId}.", userId);
                return CreatedAtAction(nameof(GetPersonById), new { id = userId }, request);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Klaida: vartotojas neprisijungęs.");
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Klaida kuriant asmens įrašą.");
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Grąžina asmens duomenis pagal ID.
        /// </summary>
        /// <param name="id">Asmens unikalus identifikatorius.</param>
        /// <returns>200 OK su asmens duomenimis.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPersonById([FromRoute, Required] Guid id)
        {
            _logger.LogInformation("Gaunamas asmens įrašas su ID {PersonId}.", id);

            try
            {
                var person = await _personService.GetPersonByIdAsync(id);

                if (person == null)
                {
                    _logger.LogWarning("Asmens įrašas su ID {PersonId} nerastas.", id);
                    return NotFound("Person not found.");
                }

                _logger.LogInformation("Asmens įrašas su ID {PersonId} sėkmingai gautas.", id);
                return Ok(new PersonResponse
                {
                    Id = person.Id,
                    Vardas = person.Vardas,
                    Pavarde = person.Pavarde,
                    AsmensKodas = person.AsmensKodas,
                    TelefonoNumeris = person.TelefonoNumeris,
                    ElPastas = person.ElPastas,
                    ProfilioNuotrauka = person.ProfilioNuotrauka != null
                        ? Convert.ToBase64String(person.ProfilioNuotrauka)
                        : null,
                    Address = new AddressResponse
                    {
                        Miestas = person.Address.Miestas,
                        Gatve = person.Address.Gatve,
                        NamoNumeris = person.Address.NamoNumeris,
                        ButoNumeris = person.Address.ButoNumeris
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Klaida gaunant asmens įrašą su ID {PersonId}.", id);
                return StatusCode(500, "An unexpected error occurred while retrieving the person.");
            }
        }

        /// <summary>
        /// Grąžina prisijungusio asmens profilį.
        /// </summary>
        /// <returns>200 OK su asmens duomenimis.</returns>
        [HttpGet("MyProfile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var person = await _personService.GetPersonByUserIdAsync(userId);

                if (person == null)
                    return NotFound("Profile not found.");

                return Ok(new PersonResponse
                {
                    Id = person.Id,
                    Vardas = person.Vardas,
                    Pavarde = person.Pavarde,
                    AsmensKodas = person.AsmensKodas,
                    TelefonoNumeris = person.TelefonoNumeris,
                    ElPastas = person.ElPastas,
                    ProfilioNuotrauka = person.ProfilioNuotrauka != null
                        ? Convert.ToBase64String(person.ProfilioNuotrauka)
                        : null,
                    Address = new AddressResponse
                    {
                        Miestas = person.Address.Miestas,
                        Gatve = person.Address.Gatve,
                        NamoNumeris = person.Address.NamoNumeris,
                        ButoNumeris = person.Address.ButoNumeris
                    }
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        #region Update Individual Fields

        /// <summary>
        /// Vardo atnaujinimas.
        /// </summary>
        /// <param name="id">Asmens ID.</param>
        /// <param name="request">Vardo atnaujinimo užklausa.</param>
        /// <returns>204 No Content, jei atnaujinimas pavyko.</returns>
        [HttpPatch("{id:guid}/UpdateVardas")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateVardas(Guid id, [FromBody, Required] UpdateVardasRequest request)
        {
            return await UpdateField(id, "Vardas", request.Vardas);
        }

        /// <summary>
        /// Pavardės atnaujinimas.
        /// </summary>
        /// <param name="id">Asmens ID.</param>
        /// <param name="request">Pavardės atnaujinimo užklausa.</param>
        /// <returns>204 No Content, jei atnaujinimas pavyko.</returns>
        [HttpPatch("{id:guid}/UpdatePavarde")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePavarde(Guid id, [FromBody, Required] UpdatePavardeRequest request)
        {
            return await UpdateField(id, "Pavarde", request.Pavarde);
        }

        /// <summary>
        /// Asmens kodo atnaujinimas.
        /// </summary>
        /// <param name="id">Asmens ID.</param>
        /// <param name="request">Asmens kodo atnaujinimo užklausa.</param>
        /// <returns>204 No Content, jei atnaujinimas pavyko.</returns>
        [HttpPatch("{id:guid}/UpdateAsmensKodas")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsmensKodas(Guid id, [FromBody, Required] UpdateAsmensKodasRequest request)
        {
            return await UpdateField(id, "AsmensKodas", request.AsmensKodas);
        }

        /// <summary>
        /// Telefono numerio atnaujinimas.
        /// </summary>
        /// <param name="id">Asmens ID.</param>
        /// <param name="request">Telefono numerio atnaujinimo užklausa.</param>
        /// <returns>204 No Content, jei atnaujinimas pavyko.</returns>
        [HttpPatch("{id:guid}/UpdateTelefonoNumeris")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateTelefonoNumeris(Guid id, [FromBody, Required] UpdateTelefonoNumerisRequest request)
        {
            return await UpdateField(id, "TelefonoNumeris", request.TelefonoNumeris);
        }

        /// <summary>
        /// El. pašto atnaujinimas.
        /// </summary>
        /// <param name="id">Asmens ID.</param>
        /// <param name="request">El. pašto atnaujinimo užklausa.</param>
        /// <returns>204 No Content, jei atnaujinimas pavyko.</returns>
        [HttpPatch("{id:guid}/UpdateElPastas")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateElPastas(Guid id, [FromBody, Required] UpdateElPastasRequest request)
        {
            return await UpdateField(id, "ElPastas", request.ElPastas);
        }

        /// <summary>
        /// Profilio nuotraukos atnaujinimas.
        /// </summary>
        /// <param name="id">Asmens ID.</param>
        /// <param name="request">Profilio nuotraukos atnaujinimo užklausa.</param>
        /// <returns>204 No Content, jei atnaujinimas pavyko.</returns>
        [HttpPatch("{id:guid}/UpdateProfilioNuotrauka")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProfilioNuotrauka(Guid id, [FromBody, Required] UpdateProfilioNuotraukaRequest request)
        {
            return await UpdateField(id, "ProfilioNuotrauka", request.ProfilioNuotrauka);
        }

        /// <summary>
        /// Adreso atnaujinimas: miestas.
        /// </summary>
        /// <param name="id">Asmens ID.</param>
        /// <param name="request">Miestas atnaujinimo užklausa.</param>
        /// <returns>204 No Content, jei atnaujinimas pavyko.</returns>
        [HttpPatch("{id:guid}/UpdateMiestas")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateMiestas(Guid id, [FromBody, Required] UpdateMiestasRequest request)
        {
            return await UpdateField(id, "Miestas", request.Miestas);
        }

        /// <summary>
        /// Adreso atnaujinimas: gatvė.
        /// </summary>
        /// <param name="id">Asmens ID.</param>
        /// <param name="request">Gatvės atnaujinimo užklausa.</param>
        /// <returns>204 No Content, jei atnaujinimas pavyko.</returns>
        [HttpPatch("{id:guid}/UpdateGatve")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateGatve(Guid id, [FromBody, Required] UpdateGatveRequest request)
        {
            return await UpdateField(id, "Gatve", request.Gatve);
        }

        /// <summary>
        /// Adreso atnaujinimas: namo numeris.
        /// </summary>
        /// <param name="id">Asmens ID.</param>
        /// <param name="request">Namo numerio atnaujinimo užklausa.</param>
        /// <returns>204 No Content, jei atnaujinimas pavyko.</returns>
        [HttpPatch("{id:guid}/UpdateNamoNumeris")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateNamoNumeris(Guid id, [FromBody, Required] UpdateNamoNumerisRequest request)
        {
            return await UpdateField(id, "NamoNumeris", request.NamoNumeris);
        }

        /// <summary>
        /// Adreso atnaujinimas: buto numeris.
        /// </summary>
        /// <param name="id">Asmens ID.</param>
        /// <param name="request">Buto numerio atnaujinimo užklausa.</param>
        /// <returns>204 No Content, jei atnaujinimas pavyko.</returns>
        [HttpPatch("{id:guid}/UpdateButoNumeris")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateButoNumeris(Guid id, [FromBody, Required] UpdateButoNumerisRequest request)
        {
            return await UpdateField(id, "ButoNumeris", request.ButoNumeris);
        }

        #endregion


        private async Task<IActionResult> UpdateField(Guid id, string fieldName, string fieldValue)
        {
            _logger.LogInformation("Pradedamas {FieldName} atnaujinimas asmens su ID {PersonId}.", fieldName, id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("{FieldName} atnaujinimo užklausa neteisinga asmeniui su ID {PersonId}.", fieldName, id);
                return BadRequest(ModelState);
            }

            try
            {
                var userId = GetCurrentUserId();
                var person = await _personService.GetPersonByIdAsync(id);

                if (person == null)
                {
                    _logger.LogWarning("Asmens įrašas su ID {PersonId} nerastas.", id);
                    return NotFound("Person not found.");
                }

                if (person.UserId != userId)
                {
                    _logger.LogWarning("Bandymas atnaujinti asmens {FieldName} su ID {PersonId} nesant savininku.", fieldName, id);
                    return Forbid("You are not allowed to update this information.");
                }

                await _personService.UpdateFieldAsync(id, fieldName, fieldValue, userId);
                _logger.LogInformation("{FieldName} sėkmingai atnaujintas asmeniui su ID {PersonId}.", fieldName, id);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Klaida: vartotojas neprisijungęs.");
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Argumento klaida atnaujinant lauką {FieldName}: {Message}", fieldName, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Netikėta klaida atnaujinant lauką {FieldName} asmeniui su ID {PersonId}.", fieldName, id);
                return StatusCode(500, "An unexpected error occurred while updating the field.");
            }
        }
    }
}
