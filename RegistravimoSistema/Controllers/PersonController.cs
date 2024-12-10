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
                UserId = user.Id, // Associate with the current user
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
        public IActionResult GetPerson(Guid id)
        {
            var person = _context.Persons
                .Select(p => new
                {
                    p.Id,
                    p.Vardas,
                    p.Pavarde,
                    p.AsmensKodas,
                    p.TelefonoNumeris,
                    p.ElPastas,
                    Address = _context.Addresses.FirstOrDefault(a => a.PersonId == p.Id)
                })
                .FirstOrDefault(p => p.Id == id);

            if (person == null)
                return NotFound("Person not found.");

            return Ok(person);
        }

        [Authorize]
        [HttpPatch("Update/{field}")]
        public IActionResult UpdatePersonalInformation([FromRoute] string field, [FromBody] UpdateFieldRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var person = _context.Persons.FirstOrDefault(p => p.UserId == userId);

            if (person == null)
                return NotFound("Your person information is not available.");

            if (string.IsNullOrWhiteSpace(request.NewValue))
                return BadRequest($"{field} cannot be empty or whitespace.");

            switch (field.ToLower())
            {
                case "vardas":
                    person.Vardas = request.NewValue;
                    break;
                case "pavarde":
                    person.Pavarde = request.NewValue;
                    break;
                case "asmenskodas":
                    person.AsmensKodas = request.NewValue;
                    break;
                case "telefononumeris":
                    person.TelefonoNumeris = request.NewValue;
                    break;
                case "elpastas":
                    person.ElPastas = request.NewValue;
                    break;
                default:
                    return BadRequest("Invalid field specified for update.");
            }

            _context.SaveChanges();
            return NoContent();
        }

        [Authorize]
        [HttpPatch("UpdateAddress/{field}")]
        public IActionResult UpdateAddressInformation([FromRoute] string field, [FromBody] UpdateFieldRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var person = _context.Persons.FirstOrDefault(p => p.UserId == userId);

            if (person == null)
                return NotFound("Your person information is not available.");

            var address = _context.Addresses.FirstOrDefault(a => a.PersonId == person.Id);
            if (address == null)
                return NotFound("Your address information is not available.");

            if (string.IsNullOrWhiteSpace(request.NewValue))
                return BadRequest($"{field} cannot be empty or whitespace.");

            switch (field.ToLower())
            {
                case "miestas":
                    address.Miestas = request.NewValue;
                    break;
                case "gatve":
                    address.Gatve = request.NewValue;
                    break;
                case "namonumeris":
                    address.NamoNumeris = request.NewValue;
                    break;
                case "butonumeris":
                    address.ButoNumeris = request.NewValue;
                    break;
                default:
                    return BadRequest("Invalid field specified for update.");
            }

            _context.SaveChanges();
            return NoContent();
        }
    }

    

    
}
