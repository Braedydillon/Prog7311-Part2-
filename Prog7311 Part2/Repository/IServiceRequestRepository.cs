using Prog7311_Part2.Models;

public interface IServiceRequestRepository
{
    Task<IEnumerable<ServiceRequest>> GetAllWithContractsAsync();
    Task<ServiceRequest?> GetByIdAsync(int id);
    Task<bool> IsContractBlocked(int contractId);
    Task AddAsync(ServiceRequest request);
    Task SaveChangesAsync();
    Task UpdateAsync(ServiceRequest serviceRequest);

    Task DeleteAsync(int id);

    Task<IEnumerable<ServiceRequest>> GetFilteredRequestsAsync(string searchString, string status);
}