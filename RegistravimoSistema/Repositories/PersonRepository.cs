using Microsoft.EntityFrameworkCore;
using RegistravimoSistema.Entities;

namespace RegistravimoSistema.Repositories
{
    public interface IPersonRepository
    {
        Task AddAsync(Person person);
        Task<Person?> GetByIdAsync(Guid id);
        Task UpdateAsync(Person person);

        //Task DeleteAsync(Guid id);
        //Task<IEnumerable<Person>> GetAllAsync();
    }

    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _context;

        public PersonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Person?> GetByIdAsync(Guid id)
        {
            return await _context.Persons.Include(p => p.Address).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Person person)
        {
            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Person person)
        {
            _context.Persons.Update(person);
            await _context.SaveChangesAsync();
        }

        //public async Task<IEnumerable<Person>> GetAllAsync()
        //{
        //    return await _context.Persons.Include(p => p.Address).ToListAsync();
        //}

        //public async Task DeleteAsync(Guid id)
        //{
        //    var person = await GetByIdAsync(id);
        //    if (person != null)
        //    {
        //        _context.Persons.Remove(person);
        //        await _context.SaveChangesAsync();
        //    }
        //}
    }
}
