using Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IProfileRepository
    {
        Task<Profile?> GetByUserIdAsync(string userId);
        Task<Profile?> GetByIdAsync(int id);
        Task<bool> AddAsync(Profile profile);
        Task<bool> UpdateAsync(Profile profile);
        Task<bool> DeleteAsync(int id);
    }
}
