using Kipas.Personel.API.Entities;

namespace Kipas.Personel.API.Interfaces
{
    public interface IAppUserRepository
    {
        Task<List<AppUser>> GetAllAsync();

        Task<AppUser?> GetByIdAsync(int id);

        Task<AppUser?> GetByUsernameAsync(
            string username);

        Task<bool> UsernameExistsAsync(
            string username);

        Task<bool> EmployeeIsLinkedAsync(
            int employeeId,
            int? excludedUserId = null);

        Task AddAsync(AppUser user);

        Task SaveChangesAsync();
    }
}