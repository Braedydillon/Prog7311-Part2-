using Microsoft.EntityFrameworkCore;
using Prog7311_Part2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prog7311_Part2.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ClientContextDatabase _context;

        public ClientRepository(ClientContextDatabase context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            return await _context.Client.ToListAsync();
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            return await _context.Client.FindAsync(id);
        }

        public async Task AddAsync(Client client)
        {
            _context.Add(client);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Client client)
        {
            _context.Update(client);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var client = await _context.Client.FindAsync(id);
            if (client != null)
            {
                _context.Client.Remove(client);
                await _context.SaveChangesAsync();
            }
        }

        public bool ClientExists(int id)
        {
            return _context.Client.Any(e => e.ClientId == id);
        }
        public async Task<IEnumerable<Client>> SearchClientsAsync(string searchString)
        {
            var query = _context.Client.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
                query = query.Where(c => c.Name.Contains(searchString));
            return await query.ToListAsync();
        }
    }
}