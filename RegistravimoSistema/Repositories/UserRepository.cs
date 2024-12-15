using Microsoft.EntityFrameworkCore;
using RegistravimoSistema.Entities;

namespace RegistravimoSistema.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task DeleteAsync(Guid id);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByUsernameAsync(string username);

        //Task<IEnumerable<User>> GetAllAsync();
        //Task UpdateAsync(User user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        //public async Task<IEnumerable<User>> GetAllAsync()
        //{
        //    return await _context.Users.ToListAsync();
        //}

        //public async Task UpdateAsync(User user)
        //{
        //    _context.Users.Update(user);
        //    await _context.SaveChangesAsync();
        //}
    }
}
