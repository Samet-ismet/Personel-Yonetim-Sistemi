using Microsoft.AspNetCore.Http;
using Kipas.Personel.API.DTOs;
using Kipas.Personel.API.Helpers;

namespace Kipas.Personel.API.Interfaces
{
    public interface IEmployeeService
    {
        Task<PagedResult<EmployeeDto>> GetAllAsync(
            EmployeeQueryParameters queryParameters);

        Task<PagedResult<EmployeeDto>?>
            GetManagerDepartmentAsync(
                int userId,
                EmployeeQueryParameters queryParameters);

        Task<EmployeeDetailDto?> GetByIdAsync(
            int id);

        Task<EmployeeDetailDto?>
            GetCurrentUserEmployeeAsync(
                int userId);

        Task<EmployeeOperationResult> AddAsync(
            CreateEmployeeDto dto);

        Task<EmployeeOperationResult> UpdateAsync(
            int id,
            UpdateEmployeeDto dto);

        Task<EmployeeOperationResult> DeleteAsync(
            int id);

        Task<EmployeeCvDto?> UploadCvAsync(
            int employeeId,
            IFormFile file,
            CancellationToken cancellationToken);

        Task<EmployeeCvDownloadResult?> DownloadCvAsync(
            int employeeId);

        Task<bool> DeleteCvAsync(
            int employeeId);
    }
}