using RegistravimoSistema.DTOs;
using RegistravimoSistema.Entities;

namespace RegistravimoSistema.Mappers
{
    public interface IAccountMapper
    {
        User MapFromDto(UserAuthRequest dto);
    }

    public class AccountMapper : IAccountMapper
    {
        public User MapFromDto(UserAuthRequest dto)
        {
            return new User
            {
                Username = dto.Username
            };
        }
    }

}