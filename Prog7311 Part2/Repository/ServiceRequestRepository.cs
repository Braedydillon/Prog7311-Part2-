using Microsoft.EntityFrameworkCore;
using Prog7311_Part2.Models;

namespace Prog7311_Part2.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly ClientContextDatabase _context;

        public ServiceRequestRepository(ClientContextDatabase context)
        {
            _context = context;
        }

        // READ ALL: Includes the Parent Contract for display in the Index
        public async Task<IEnumerable<ServiceRequest>> GetAllWithContractsAsync()
            => await _context.ServiceRequest
                             .Include(s => s.Contract)
                             .ThenInclude(c => c.Client) // Optional: includes client name too
                             .ToListAsync();

        // READ ONE: Detailed view for Edit/Details pages
        public async Task<ServiceRequest?> GetByIdAsync(int id)
            => await _context.ServiceRequest
                             .Include(s => s.Contract)
                             .FirstOrDefaultAsync(m => m.ServiceRequestId == id);

        // BUSINESS RULE: This matches your UML "State Management"
        public async Task<bool> IsContractBlocked(int contractId)
        {
            var contract = await _context.Contract.FindAsync(contractId);
            // Returns true if contract is missing, expired, or on hold
            return contract == null ||
                   contract.Status == ContractStatus.Expired ||
                   contract.Status == ContractStatus.OnHold;
        }

        // CREATE
        public async Task AddAsync(ServiceRequest request)
        {
            await _context.ServiceRequest.AddAsync(request);
        }

        // UPDATE
        public async Task UpdateAsync(ServiceRequest request)
        {
            _context.Entry(request).State = EntityState.Modified;
            // Or use _context.Update(request);
        }

        // DELETE
        public async Task DeleteAsync(int id)
        {
            var request = await _context.ServiceRequest.FindAsync(id);
            if (request != null)
            {
                _context.ServiceRequest.Remove(request);
            }
        }

        // SAVE: Returns true if any changes were written to the DB
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        Task IServiceRequestRepository.SaveChangesAsync()
        {
            return SaveChangesAsync();
        }

        public async Task<IEnumerable<ServiceRequest>> GetFilteredRequestsAsync(string searchString, string status)
        {
            var query = _context.ServiceRequest.Include(s => s.Contract).AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
                query = query.Where(s => s.Description.Contains(searchString));
            if (!string.IsNullOrEmpty(status))
                query = query.Where(s => s.Status == status);
            return await query.ToListAsync();
        }
    }
}