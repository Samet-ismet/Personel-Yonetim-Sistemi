using Kipas.Personel.API.DTOs;
using Kipas.Personel.API.Entities;

namespace Kipas.Personel.API.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<(List<Employee> Items, int TotalCount)>
            GetPagedAsync(
                EmployeeQueryParameters queryParameters);

        Task<Employee?> GetByIdAsync(
            int id);

        Task<bool> HasLinkedUserAsync(
            int employeeId);

        Task AddAsync(
            Employee employee);

        Task UpdateAsync(
            Employee employee);

        Task DeleteAsync(
            Employee employee);

        Task SaveChangesAsync();
    }
}