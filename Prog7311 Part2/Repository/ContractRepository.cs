using Microsoft.EntityFrameworkCore;
using Prog7311_Part2.Models;

namespace Prog7311_Part2.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly ClientContextDatabase _context;

        public ContractRepository(ClientContextDatabase context) => _context = context;

        public async Task<IEnumerable<Contract>> GetAllAsync()
            => await _context.Contract.Include(c => c.Client).ToListAsync();

        public async Task<Contract?> GetByIdAsync(int id)
            => await _context.Contract.Include(c => c.Client).FirstOrDefaultAsync(m => m.ContractId == id);

        public async Task AddAsync(Contract contract)
        {
            _context.Add(contract);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Contract contract)
        {
            _context.Update(contract);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var contract = await _context.Contract.FindAsync(id);
            if (contract != null)
            {
                _context.Contract.Remove(contract);
                await _context.SaveChangesAsync();
            }
        }

        public bool ContractExists(int id) => _context.Contract.Any(e => e.ContractId == id);

        // Required by IContractRepository for the ServiceRequest dropdowns
        public async Task<IEnumerable<Contract>> GetAllWithClientsAsync()
        {
            return await _context.Contract.Include(c => c.Client).ToListAsync();
        }

        // Required by IContractRepository for the Admin Filter/Search
        public async Task<IEnumerable<Contract>> GetFilteredAsync(DateTime? start, DateTime? end, ContractStatus? status)
        {
            var query = _context.Contract.Include(c => c.Client).AsQueryable();

            if (status.HasValue) query = query.Where(c => c.Status == status);

            // Note: If your model uses DateOnly, keep these conversions. 
            // If your model uses DateTime, remove the 'DateOnly.FromDateTime' part.
            if (start.HasValue) query = query.Where(c => c.StartDate >= DateOnly.FromDateTime(start.Value));
            if (end.HasValue) query = query.Where(c => c.EndDate <= DateOnly.FromDateTime(end.Value));

            return await query.ToListAsync();
        }

   
    }
}