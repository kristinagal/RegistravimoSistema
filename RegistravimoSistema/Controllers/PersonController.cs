using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistravimoSistema.DTOs;
using RegistravimoSistema.Entities;
using RegistravimoSistema.Services;
using System.Security.Claims;

namespace RegistravimoSistema.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
                throw new UnauthorizedAccessException("User must be authenticated.");
            return parsedUserId;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePerson([FromBody] PersonRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetCurrentUserId();

                // Check if the user already has a Person entity
                var existingPerson = await _personService.GetPersonByUserIdAsync(userId);
                if (existingPerson != null)
                {
                    return Conflict("You have already created a person. A user can only create one person.");
                }

                await _personService.CreatePersonAsync(request, userId);
                return CreatedAtAction(nameof(GetPersonById), new { id = userId }, request);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPersonById(Guid id)
        {
            try
            {
                var person = await _personService.GetPersonByIdAsync(id);

                if (person == null)
                    return NotFound("Person not found.");

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
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred while retrieving the person.");
            }
        }

        #region Update Individual Fields

        [HttpPatch("{id:guid}/UpdateVardas")]
        public async Task<IActionResult> UpdateVardas(Guid id, [FromBody] UpdateVardasRequest request)
        {
            return await UpdateField(id, "Vardas", request.Vardas);
        }

        [HttpPatch("{id:guid}/UpdatePavarde")]
        public async Task<IActionResult> UpdatePavarde(Guid id, [FromBody] UpdatePavardeRequest request)
        {
            return await UpdateField(id, "Pavarde", request.Pavarde);
        }

        [HttpPatch("{id:guid}/UpdateAsmensKodas")]
        public async Task<IActionResult> UpdateAsmensKodas(Guid id, [FromBody] UpdateAsmensKodasRequest request)
        {
            return await UpdateField(id, "AsmensKodas", request.AsmensKodas);
        }

        [HttpPatch("{id:guid}/UpdateTelefonoNumeris")]
        public async Task<IActionResult> UpdateTelefonoNumeris(Guid id, [FromBody] UpdateTelefonoNumerisRequest request)
        {
            return await UpdateField(id, "TelefonoNumeris", request.TelefonoNumeris);
        }

        [HttpPatch("{id:guid}/UpdateElPastas")]
        public async Task<IActionResult> UpdateElPastas(Guid id, [FromBody] UpdateElPastasRequest request)
        {
            return await UpdateField(id, "ElPastas", request.ElPastas);
        }

        [HttpPatch("{id:guid}/UpdateProfilioNuotrauka")]
        public async Task<IActionResult> UpdateProfilioNuotrauka(Guid id, [FromBody] UpdateProfilioNuotraukaRequest request)
        {
            return await UpdateField(id, "ProfilioNuotrauka", request.ProfilioNuotrauka);
        }

        [HttpPatch("{id:guid}/UpdateMiestas")]
        public async Task<IActionResult> UpdateMiestas(Guid id, [FromBody] UpdateMiestasRequest request)
        {
            return await UpdateField(id, "Miestas", request.Miestas);
        }

        [HttpPatch("{id:guid}/UpdateGatve")]
        public async Task<IActionResult> UpdateGatve(Guid id, [FromBody] UpdateGatveRequest request)
        {
            return await UpdateField(id, "Gatve", request.Gatve);
        }

        [HttpPatch("{id:guid}/UpdateNamoNumeris")]
        public async Task<IActionResult> UpdateNamoNumeris(Guid id, [FromBody] UpdateNamoNumerisRequest request)
        {
            return await UpdateField(id, "NamoNumeris", request.NamoNumeris);
        }

        [HttpPatch("{id:guid}/UpdateButoNumeris")]
        public async Task<IActionResult> UpdateButoNumeris(Guid id, [FromBody] UpdateButoNumerisRequest request)
        {
            return await UpdateField(id, "ButoNumeris", request.ButoNumeris);
        }

        #endregion

        private async Task<IActionResult> UpdateField(Guid id, string fieldName, string fieldValue)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetCurrentUserId();
                var person = await _personService.GetPersonByIdAsync(id);

                if (person == null)
                    return NotFound("Person not found.");

                if (person.UserId != userId)
                    return Forbid("You are not allowed to update this information.");

                await _personService.UpdateFieldAsync(id, fieldName, fieldValue, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred while updating the field.");
            }
        }

    }
}
