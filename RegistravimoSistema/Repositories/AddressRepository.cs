using Microsoft.EntityFrameworkCore;
using RegistravimoSistema.Entities;

namespace RegistravimoSistema.Repositories
{
    public interface IAddressRepository
    {
        Task AddAsync(Address address);
        Task UpdateAsync(Address address);

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

        public async Task UpdateAsync(Address address)
        {
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
        }

    }
}
