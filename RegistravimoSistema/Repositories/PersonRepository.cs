using Microsoft.EntityFrameworkCore;
using RegistravimoSistema.Entities;

namespace RegistravimoSistema.Repositories
{
    public interface IPersonRepository
    {
        Task AddAsync(Person person);
        Task<Person?> GetByIdAsync(Guid id);
        Task UpdateAsync(Person person);
        Task<Person?> GetByUserIdAsync(Guid userId);
        Task DeleteAsync(Guid personId);
    }

    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _context;

        public PersonRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(Person person)
        {
            if (person == null) throw new ArgumentNullException(nameof(person), "Person cannot be null.");

            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();
        }

        public async Task<Person?> GetByIdAsync(Guid id)
        {
            return await _context.Persons.Include(p => p.Address).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(Person person)
        {
            if (person == null) throw new ArgumentNullException(nameof(person), "Person cannot be null.");

            _context.Persons.Update(person);
            await _context.SaveChangesAsync();
        }

        public async Task<Person?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Persons
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task DeleteAsync(Guid personId)
        {
            var person = await _context.Persons.FindAsync(personId);
            if (person == null) throw new ArgumentException($"Person with ID {personId} not found.");

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();
        }
    }
}
