using Kipas.Personel.API.DTOs;
using Kipas.Personel.API.Helpers;

namespace Kipas.Personel.API.Interfaces
{
    public interface IUserManagementService
    {
        Task<List<UserDto>> GetAllAsync();

        Task<UserDto?> GetByIdAsync(
            int id);

        Task<UserManagementOperationResult>
            CreateAsync(
             CreateUserDto dto);

        Task<UserManagementOperationResult>
            UpdateAccessAsync(
                int id,
                UpdateUserAccessDto dto);

        Task<UserManagementOperationResult>
            UpdateStatusAsync(
                int id,
                int currentUserId,
                UpdateUserStatusDto dto);
    }
}