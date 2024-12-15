using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistravimoSistema.DTOs;
using RegistravimoSistema.Repositories;
using RegistravimoSistema.Services;
using System.Security.Claims;

namespace RegistravimoSistema.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;
        private readonly IPersonRepository _personRepository;

        public PersonController(IPersonService personService, IPersonRepository personRepository)
        {
            _personService = personService;
            _personRepository = personRepository;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePerson([FromBody] PersonRequest request)
        {
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId) || !Guid.TryParse(currentUserId, out var userId))
                return Unauthorized("User must be logged in.");

            await _personService.CreatePersonAsync(request, userId);

            return Created("", new { message = "Person created successfully!" });
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPersonById(Guid id)
        {
            var person = await _personRepository.GetByIdAsync(id);

            if (person == null)
                return NotFound("Person not found.");

            var response = new PersonResponse
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
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePerson(Guid id, [FromBody] PersonRequest request)
        {
            var person = await _personRepository.GetByIdAsync(id);
            if (person == null)
                return NotFound("Person not found.");

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var userId) || (person.UserId != userId && !User.IsInRole("Admin")))
                return Forbid("You do not have permission to update this person.");

            // Update person details
            if (!string.IsNullOrWhiteSpace(request.Vardas))
                person.Vardas = request.Vardas;

            if (!string.IsNullOrWhiteSpace(request.Pavarde))
                person.Pavarde = request.Pavarde;

            if (!string.IsNullOrWhiteSpace(request.AsmensKodas))
                person.AsmensKodas = request.AsmensKodas;

            if (!string.IsNullOrWhiteSpace(request.TelefonoNumeris))
                person.TelefonoNumeris = request.TelefonoNumeris;

            if (!string.IsNullOrWhiteSpace(request.ElPastas))
                person.ElPastas = request.ElPastas;

            if (!string.IsNullOrWhiteSpace(request.ProfilioNuotrauka))
            {
                try
                {
                    person.ProfilioNuotrauka = Convert.FromBase64String(request.ProfilioNuotrauka);
                }
                catch (FormatException)
                {
                    return BadRequest("Invalid profile picture format. Ensure it is Base64 encoded.");
                }
            }

            await _personRepository.UpdateAsync(person);

            return Ok("Person updated successfully!");
        }
    }
}
