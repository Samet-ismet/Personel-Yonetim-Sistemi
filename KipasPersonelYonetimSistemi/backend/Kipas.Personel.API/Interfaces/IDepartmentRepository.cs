using Kipas.Personel.API.Entities;

namespace Kipas.Personel.API.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetAllAsync(
            bool includeInactive);

        Task<Department?> GetByIdAsync(int id);

        Task<bool> NameExistsAsync(
            string name,
            int? excludedDepartmentId = null);

        Task AddAsync(Department department);

        Task SaveChangesAsync();
    }
}