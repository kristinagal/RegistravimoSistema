using RegistravimoSistema.DTOs;
using RegistravimoSistema.Entities;
using RegistravimoSistema.Mappers;
using RegistravimoSistema.Repositories;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace RegistravimoSistema.Services;

public interface IPersonService
{
    Task<Person?> GetPersonByIdAsync(Guid id);
    Task<Person?> GetPersonByUserIdAsync(Guid userId);
    Task CreatePersonAsync(PersonRequest request, Guid userId);
    Task UpdateFieldAsync(Guid personId, string fieldName, string fieldValue, Guid userId);
    Task UpdateAddressFieldAsync(Guid personId, string fieldName, string fieldValue, Guid userId);
}

public class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IPersonMapper _mapper;

    public PersonService(IPersonRepository personRepository, IAddressRepository addressRepository, IPersonMapper mapper)
    {
        _personRepository = personRepository;
        _addressRepository = addressRepository;
        _mapper = mapper;
    }

    public async Task<Person?> GetPersonByIdAsync(Guid id)
    {
        return await _personRepository.GetByIdAsync(id);
    }

    public async Task<Person?> GetPersonByUserIdAsync(Guid userId)
    {
        return await _personRepository.GetByUserIdAsync(userId);
    }

    public async Task CreatePersonAsync(PersonRequest request, Guid userId)
    {
        // Ensure the profile picture is valid and resize it
        request.ProfilioNuotrauka = ProcessProfilePhoto(request.ProfilioNuotrauka);

        // Map the Person and Address entities from the request
        var person = _mapper.MapFromDto(request, userId);
        var address = _mapper.MapAddressFromDto(request, person.Id);

        // Save the Person and Address entities to the database
        await _personRepository.AddAsync(person);
        await _addressRepository.AddAsync(address);
    }

    public async Task UpdateFieldAsync(Guid personId, string fieldName, string fieldValue, Guid userId)
    {
        var person = await _personRepository.GetByIdAsync(personId);
        if (person == null)
            throw new ArgumentException("Person not found.");

        try
        {
            switch (fieldName)
            {
                case "Vardas":
                    person.Vardas = fieldValue;
                    break;
                case "Pavarde":
                    person.Pavarde = fieldValue;
                    break;
                case "TelefonoNumeris":
                    person.TelefonoNumeris = fieldValue;
                    break;
                case "ElPastas":
                    person.ElPastas = fieldValue;
                    break;
                default:
                    throw new ArgumentException("Invalid field name."); // Directly throw ArgumentException
            }

            await _personRepository.UpdateAsync(person);
        }
        catch (ArgumentException) // Catch but rethrow ArgumentException
        {
            //_logger.LogError($"Failed to update field: {fieldName}");
            throw;
        }
        catch (Exception ex)
        {
            //_logger.LogError($"An unexpected error occurred: {ex.Message}");
            throw new Exception("An unexpected error occurred while updating the field.", ex);
        }
    }

    //public async Task UpdateFieldAsync(Guid personId, string fieldName, string fieldValue, Guid userId)
    //{
    //    if (string.IsNullOrWhiteSpace(fieldValue))
    //        throw new ArgumentException($"{fieldName} cannot be empty.");

    //    var person = await _personRepository.GetByIdAsync(personId);
    //    if (person == null) throw new Exception("Person not found.");
    //    if (person.UserId != userId)
    //        throw new UnauthorizedAccessException("You do not have permission to update this field.");

    //    try
    //    {
    //        switch (fieldName)
    //        {
    //            case "Vardas": person.Vardas = fieldValue; break;
    //            case "Pavarde": person.Pavarde = fieldValue; break;
    //            case "AsmensKodas": person.AsmensKodas = fieldValue; break;
    //            case "TelefonoNumeris": person.TelefonoNumeris = fieldValue; break;
    //            case "ElPastas": person.ElPastas = fieldValue; break;

    //            case "ProfilioNuotrauka":
    //                // Use the same logic as in CreatePersonAsync
    //                person.ProfilioNuotrauka = Convert.FromBase64String(
    //                    ProcessProfilePhoto(fieldValue)
    //                );
    //                break;

    //            default:
    //                throw new ArgumentException("Invalid field name.");
    //        }

    //        await _personRepository.UpdateAsync(person);
    //    }
    //    catch (FormatException)
    //    {
    //        throw new ArgumentException("Invalid profile picture format. Ensure the image is Base64-encoded.");
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new Exception($"Failed to update {fieldName}: {ex.Message}", ex);
    //    }
    //}

    public async Task UpdateAddressFieldAsync(Guid personId, string fieldName, string fieldValue, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(fieldValue))
            throw new ArgumentException($"{fieldName} cannot be empty.");

        var person = await _personRepository.GetByIdAsync(personId);
        if (person == null) throw new Exception("Person not found.");
        if (person.UserId != userId)
            throw new UnauthorizedAccessException("You do not have permission to update this field.");

        var address = person.Address;
        if (address == null) throw new Exception("Address not found.");

        switch (fieldName)
        {
            case "Miestas": address.Miestas = fieldValue; break;
            case "Gatve": address.Gatve = fieldValue; break;
            case "NamoNumeris": address.NamoNumeris = fieldValue; break;
            case "ButoNumeris": address.ButoNumeris = fieldValue; break;
            default: throw new ArgumentException("Invalid address field name.");
        }

        await _addressRepository.UpdateAsync(address);
    }

    private byte[] ResizeImage(byte[] imageBytes)
    {
        using var image = Image.Load(imageBytes);
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(200, 200),
            Mode = ResizeMode.Stretch
        }));

        using var output = new MemoryStream();
        image.SaveAsJpeg(output);
        return output.ToArray();
    }

    private string ProcessProfilePhoto(string? base64Image)
    {
        if (string.IsNullOrWhiteSpace(base64Image))
            throw new ArgumentException("Profile picture is required.");

        try
        {
            // Convert and resize the image
            var imageBytes = Convert.FromBase64String(base64Image);
            var resizedImageBytes = ResizeImage(imageBytes);

            // Convert resized image back to Base64
            return Convert.ToBase64String(resizedImageBytes);
        }
        catch (FormatException)
        {
            throw new ArgumentException("Invalid profile picture format. Ensure it is Base64 encoded.");
        }
    }
}
