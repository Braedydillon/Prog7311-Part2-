using Prog7311_Part2.Models;

namespace Prog7311_Part2.Repositories
{
    public interface IContractRepository
    {
        Task<IEnumerable<Contract>> GetAllAsync();
        Task<Contract?> GetByIdAsync(int id);
        Task AddAsync(Contract contract);
        Task UpdateAsync(Contract contract);
        Task DeleteAsync(int id);
        bool ContractExists(int id);

        Task<IEnumerable<Contract>> GetFilteredAsync(DateTime? start, DateTime? end, ContractStatus? status);
        Task<IEnumerable<Contract>> GetAllWithClientsAsync();


    }
}