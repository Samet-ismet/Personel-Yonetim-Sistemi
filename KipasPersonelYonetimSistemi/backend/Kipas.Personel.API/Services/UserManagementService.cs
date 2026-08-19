using Microsoft.AspNetCore.Identity;
using Kipas.Personel.API.DTOs;
using Kipas.Personel.API.Entities;
using Kipas.Personel.API.Helpers;
using Kipas.Personel.API.Interfaces;

namespace Kipas.Personel.API.Services
{
    public class UserManagementService
        : IUserManagementService
    {
        private readonly IAppUserRepository
            _userRepository;

        private readonly IEmployeeRepository
            _employeeRepository;

        private readonly IPasswordHasher<AppUser>
            _passwordHasher;

        private readonly ILogger<UserManagementService>
            _logger;


        public UserManagementService(
            IAppUserRepository userRepository,
            IEmployeeRepository employeeRepository,
            IPasswordHasher<AppUser> passwordHasher,
            ILogger<UserManagementService> logger)
        {
            _userRepository =
                userRepository;

            _employeeRepository =
                employeeRepository;

            _passwordHasher =
                passwordHasher;

            _logger =
                logger;
        }


        public async Task<List<UserDto>>
            GetAllAsync()
        {
            var users =
                await _userRepository
                    .GetAllAsync();

            return users
                .Select(MapToDto)
                .ToList();
        }


        public async Task<UserDto?>
            GetByIdAsync(
                int id)
        {
            var user =
                await _userRepository
                    .GetByIdAsync(id);

            return user == null
                ? null
                : MapToDto(user);
        }


        public async Task<UserManagementOperationResult>
            CreateAsync(
                CreateUserDto dto)
        {
            var normalizedUsername =
                dto.Username
                    .Trim()
                    .ToLowerInvariant();


            var usernameExists =
                await _userRepository
                    .UsernameExistsAsync(
                        normalizedUsername);

            if (usernameExists)
            {
                _logger.LogWarning(
                    "Kullanıcı oluşturma işlemi başarısız. Kullanıcı adı zaten mevcut: {Username}",
                    normalizedUsername);

                return new()
                {
                    Status =
                        UserManagementOperationStatus
                            .UsernameAlreadyExists
                };
            }


            if (!RoleNames.TryNormalize(
                    dto.Role,
                    out var normalizedRole))
            {
                return new()
                {
                    Status =
                        UserManagementOperationStatus
                            .InvalidRole
                };
            }


            Employee? employee =
                null;


            if (dto.EmployeeId.HasValue)
            {
                employee =
                    await _employeeRepository
                        .GetByIdAsync(
                            dto.EmployeeId.Value);

                if (employee == null)
                {
                    return new()
                    {
                        Status =
                            UserManagementOperationStatus
                                .EmployeeNotFound
                    };
                }


                if (!employee.IsActive)
                {
                    return new()
                    {
                        Status =
                            UserManagementOperationStatus
                                .EmployeeInactive
                    };
                }


                var alreadyLinked =
                    await _userRepository
                        .EmployeeIsLinkedAsync(
                            employee.Id);

                if (alreadyLinked)
                {
                    return new()
                    {
                        Status =
                            UserManagementOperationStatus
                                .EmployeeAlreadyLinked
                    };
                }
            }


            var user =
                new AppUser
                {
                    Username =
                        normalizedUsername,

                    Role =
                        normalizedRole,

                    IsActive =
                        true,

                    CreatedAt =
                        DateTime.UtcNow,

                    EmployeeId =
                        employee?.Id,

                    Employee =
                        employee
                };


            user.PasswordHash =
                _passwordHasher
                    .HashPassword(
                        user,
                        dto.Password);


            await _userRepository
                .AddAsync(user);

            await _userRepository
                .SaveChangesAsync();


            _logger.LogInformation(
                "Yeni kullanıcı Admin tarafından oluşturuldu. UserId: {UserId}, Role: {Role}, EmployeeId: {EmployeeId}",
                user.Id,
                user.Role,
                user.EmployeeId);


            return new()
            {
                Status =
                    UserManagementOperationStatus
                        .Success,

                User =
                    MapToDto(user)
            };
        }


        public async Task<UserManagementOperationResult>
            UpdateAccessAsync(
                int id,
                UpdateUserAccessDto dto)
        {
            var user =
                await _userRepository
                    .GetByIdAsync(id);

            if (user == null)
            {
                return new()
                {
                    Status =
                        UserManagementOperationStatus
                            .UserNotFound
                };
            }


            if (!RoleNames.TryNormalize(
                    dto.Role,
                    out var normalizedRole))
            {
                return new()
                {
                    Status =
                        UserManagementOperationStatus
                            .InvalidRole
                };
            }


            Employee? employee =
                null;


            if (dto.EmployeeId.HasValue)
            {
                employee =
                    await _employeeRepository
                        .GetByIdAsync(
                            dto.EmployeeId.Value);

                if (employee == null)
                {
                    return new()
                    {
                        Status =
                            UserManagementOperationStatus
                                .EmployeeNotFound
                    };
                }


                if (!employee.IsActive)
                {
                    return new()
                    {
                        Status =
                            UserManagementOperationStatus
                                .EmployeeInactive
                    };
                }


                var alreadyLinked =
                    await _userRepository
                        .EmployeeIsLinkedAsync(
                            employee.Id,
                            user.Id);

                if (alreadyLinked)
                {
                    return new()
                    {
                        Status =
                            UserManagementOperationStatus
                                .EmployeeAlreadyLinked
                    };
                }
            }


            user.Role =
                normalizedRole;

            user.EmployeeId =
                employee?.Id;

            user.Employee =
                employee;


            await _userRepository
                .SaveChangesAsync();


            _logger.LogInformation(
                "Kullanıcı erişimi güncellendi. UserId: {UserId}, Role: {Role}, EmployeeId: {EmployeeId}",
                user.Id,
                user.Role,
                user.EmployeeId);


            return new()
            {
                Status =
                    UserManagementOperationStatus
                        .Success,

                User =
                    MapToDto(user)
            };
        }


        public async Task<UserManagementOperationResult>
            UpdateStatusAsync(
                int id,
                int currentUserId,
                UpdateUserStatusDto dto)
        {
            var user =
                await _userRepository
                    .GetByIdAsync(id);

            if (user == null)
            {
                return new()
                {
                    Status =
                        UserManagementOperationStatus
                            .UserNotFound
                };
            }


            if (
                user.Id == currentUserId &&
                !dto.IsActive
            )
            {
                return new()
                {
                    Status =
                        UserManagementOperationStatus
                            .CannotDeactivateSelf
                };
            }


            user.IsActive =
                dto.IsActive;


            await _userRepository
                .SaveChangesAsync();


            _logger.LogInformation(
                "Kullanıcı aktiflik durumu güncellendi. UserId: {UserId}, IsActive: {IsActive}",
                user.Id,
                user.IsActive);


            return new()
            {
                Status =
                    UserManagementOperationStatus
                        .Success,

                User =
                    MapToDto(user)
            };
        }


        private static UserDto MapToDto(
            AppUser user)
        {
            return new UserDto
            {
                Id =
                    user.Id,

                Username =
                    user.Username,

                Role =
                    user.Role,

                IsActive =
                    user.IsActive,

                EmployeeId =
                    user.EmployeeId,

                EmployeeName =
                    user.Employee == null
                        ? null
                        : $"{user.Employee.FirstName} " +
                          $"{user.Employee.LastName}",

                DepartmentName =
                    user.Employee?
                        .Department?
                        .Name,

                CreatedAt =
                    user.CreatedAt
            };
        }
    }
}