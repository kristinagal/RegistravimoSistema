using System.Text;
using RegistravimoSistema.Repositories;

namespace RegistravimoSistema.Services
{
    public interface IAccountService
    {
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
        Task DeleteUserWithPersonAsync(Guid userId);
    }

    public class AccountService : IAccountService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IUserRepository _userRepository;

        public AccountService(IPersonRepository personRepository, IUserRepository userRepository)
        {
            _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }

        public async Task DeleteUserWithPersonAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));

            // Find and delete Person
            var person = await _personRepository.GetByUserIdAsync(userId);
            if (person != null)
            {
                await _personRepository.DeleteAsync(person.Id);
            }

            // Delete User
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            await _userRepository.DeleteAsync(userId);
        }
    }
}
