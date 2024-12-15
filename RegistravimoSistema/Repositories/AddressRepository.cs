using Microsoft.EntityFrameworkCore;
using RegistravimoSistema.Entities;

namespace RegistravimoSistema.Repositories
{
    public interface IAddressRepository
    {
        Task AddAsync(Address address);

        //Task<IEnumerable<Address>> GetAllAsync();
        //Task DeleteAsync(Guid id);
        //Task<IEnumerable<Address>> GetByCityAsync(string city);
        //Task<Address?> GetByIdAsync(Guid id);
        //Task UpdateAsync(Address address);
    }

    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext _context;

        public AddressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Address address)
        {
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();
        }

        //public async Task<IEnumerable<Address>> GetAllAsync()
        //{
        //    return await _context.Addresses.ToListAsync();
        //}

        //public async Task<Address?> GetByIdAsync(Guid id)
        //{
        //    return await _context.Addresses.FindAsync(id);
        //}

        //public async Task UpdateAsync(Address address)
        //{
        //    _context.Addresses.Update(address);
        //}

        //    await _context.SaveChangesAsync();
        //public async Task DeleteAsync(Guid id)
        //{
        //    var address = await GetByIdAsync(id);
        //    if (address != null)
        //    {
        //        _context.Addresses.Remove(address);
        //        await _context.SaveChangesAsync();
        //    }
        //}

        //public async Task<IEnumerable<Address>> GetByCityAsync(string city)
        //{
        //    return await _context.Addresses.Where(a => a.Miestas == city).ToListAsync();
        //}
    }
}
