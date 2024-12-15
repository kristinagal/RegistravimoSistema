using RegistravimoSistema.DTOs;
using RegistravimoSistema.Entities;

namespace RegistravimoSistema.Mappers
{
    public interface IPersonMapper
    {
        Person MapFromDto(PersonRequest dto, Guid userId);
        Address MapAddressFromDto(PersonRequest dto, Guid personId);
    }

    public class PersonMapper : IPersonMapper
    {
        public Person MapFromDto(PersonRequest dto, Guid userId)
        {
            return new Person
            {
                UserId = userId,
                Vardas = dto.Vardas,
                Pavarde = dto.Pavarde,
                AsmensKodas = dto.AsmensKodas,
                TelefonoNumeris = dto.TelefonoNumeris,
                ElPastas = dto.ElPastas,
                ProfilioNuotrauka = !string.IsNullOrWhiteSpace(dto.ProfilioNuotrauka)
                    ? Convert.FromBase64String(dto.ProfilioNuotrauka)
                    : Array.Empty<byte>()
            };
        }

        public Address MapAddressFromDto(PersonRequest dto, Guid personId)
        {
            return new Address
            {
                PersonId = personId,
                Miestas = dto.Miestas,
                Gatve = dto.Gatve,
                NamoNumeris = dto.NamoNumeris,
                ButoNumeris = dto.ButoNumeris
            };
        }
    }
}