using Kipas.Personel.API.DTOs;
using Kipas.Personel.API.Helpers;

namespace Kipas.Personel.API.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<DepartmentDto>> GetAllAsync(
            bool includeInactive);

        Task<DepartmentDto?> GetByIdAsync(int id);

        Task<DepartmentOperationResult> AddAsync(
            CreateDepartmentDto dto);

        Task<DepartmentOperationResult> UpdateAsync(
            int id,
            UpdateDepartmentDto dto);

        Task<DepartmentOperationStatus> DeactivateAsync(
            int id);
    }
}