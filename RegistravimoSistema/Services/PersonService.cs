using RegistravimoSistema.DTOs;
using RegistravimoSistema.Mappers;
using RegistravimoSistema.Repositories;

namespace RegistravimoSistema.Services
{
    public interface IPersonService
    {
        Task CreatePersonAsync(PersonRequest request, Guid userId);
    }

    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IPersonMapper _personMapper;

        public PersonService(
            IPersonRepository personRepository,
            IAddressRepository addressRepository,
            IPersonMapper personMapper)
        {
            _personRepository = personRepository;
            _addressRepository = addressRepository;
            _personMapper = personMapper;
        }

        public async Task CreatePersonAsync(PersonRequest request, Guid userId)
        {
            var person = _personMapper.MapFromDto(request, userId);
            var address = _personMapper.MapAddressFromDto(request, person.Id);

            await _personRepository.AddAsync(person);
            await _addressRepository.AddAsync(address);
        }
    }
}
