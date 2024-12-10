using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistravimoSistema.Entities;
using RegistravimoSistema.DTOs;
using System.Security.Claims;

namespace RegistravimoSistema.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PersonController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public IActionResult CreatePerson([FromBody] CreatePersonRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Vardas) || string.IsNullOrWhiteSpace(request.Pavarde))
                return BadRequest("Vardas and Pavarde are required.");

            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId) || !Guid.TryParse(currentUserId, out var userId))
                return Unauthorized("User must be logged in.");

            var user = _context.Users.Find(userId);
            if (user == null)
                return Unauthorized("Invalid user.");

            byte[]? profilePictureBytes = null;

            if (!string.IsNullOrWhiteSpace(request.ProfilioNuotrauka))
            {
                try
                {
                    profilePictureBytes = Convert.FromBase64String(request.ProfilioNuotrauka);
                }
                catch (FormatException)
                {
                    return BadRequest("Invalid profile picture format. Ensure it is Base64 encoded.");
                }
            }

            var person = new Person
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Vardas = request.Vardas,
                Pavarde = request.Pavarde,
                AsmensKodas = request.AsmensKodas,
                TelefonoNumeris = request.TelefonoNumeris,
                ElPastas = request.ElPastas,
                ProfilioNuotrauka = profilePictureBytes ?? Array.Empty<byte>()
            };

            _context.Persons.Add(person);
            _context.SaveChanges();

            var address = new Address
            {
                Id = Guid.NewGuid(),
                PersonId = person.Id,
                Miestas = request.Miestas,
                Gatve = request.Gatve,
                NamoNumeris = request.NamoNumeris,
                ButoNumeris = request.ButoNumeris
            };

            _context.Addresses.Add(address);
            _context.SaveChanges();

            return Created("", new { PersonId = person.Id });
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public IActionResult GetPersonById(Guid id)
        {
            var person = _context.Persons
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.Vardas,
                    p.Pavarde,
                    p.AsmensKodas,
                    p.TelefonoNumeris,
                    p.ElPastas,
                    Address = new
                    {
                        p.Address.Miestas,
                        p.Address.Gatve,
                        p.Address.NamoNumeris,
                        p.Address.ButoNumeris
                    }
                })
                .FirstOrDefault();

            if (person == null)
                return NotFound("Person not found.");

            return Ok(person);
        }

        [Authorize]
        [HttpPut("{id:guid}")]
        public IActionResult UpdatePerson(Guid id, [FromBody] UpdatePersonRequest request)
        {
            var person = _context.Persons.FirstOrDefault(p => p.Id == id);
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

            _context.SaveChanges();

            // Update address fields
            if (!string.IsNullOrWhiteSpace(request.Miestas))
                UpdateAddressField(person.Id, "miestas", request.Miestas);

            if (!string.IsNullOrWhiteSpace(request.Gatve))
                UpdateAddressField(person.Id, "gatve", request.Gatve);

            if (!string.IsNullOrWhiteSpace(request.NamoNumeris))
                UpdateAddressField(person.Id, "namonumeris", request.NamoNumeris);

            if (!string.IsNullOrWhiteSpace(request.ButoNumeris))
                UpdateAddressField(person.Id, "butonumeris", request.ButoNumeris);

            return Ok("Person updated successfully!");
        }

        private void UpdateAddressField(Guid personId, string field, string value)
        {
            var address = _context.Addresses.FirstOrDefault(a => a.PersonId == personId);
            if (address != null)
            {
                switch (field.ToLower())
                {
                    case "miestas":
                        address.Miestas = value;
                        break;
                    case "gatve":
                        address.Gatve = value;
                        break;
                    case "namonumeris":
                        address.NamoNumeris = value;
                        break;
                    case "butonumeris":
                        address.ButoNumeris = value;
                        break;
                }
                _context.SaveChanges();
            }
        }
    }
}
