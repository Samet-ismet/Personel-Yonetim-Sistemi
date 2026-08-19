using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kipas.Personel.API.DTOs;
using Kipas.Personel.API.Helpers;
using Kipas.Personel.API.Interfaces;
using System.Security.Claims;

namespace Kipas.Personel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = RoleNames.Admin)]
    public class UserManagementController
        : ControllerBase
    {
        private readonly IUserManagementService
            _userManagementService;


        public UserManagementController(
            IUserManagementService
                userManagementService)
        {
            _userManagementService =
                userManagementService;
        }


        [HttpGet]
        public async Task<IActionResult>
            GetAll()
        {
            var users =
                await _userManagementService
                    .GetAllAsync();

            return Ok(
                new ApiResponse<List<UserDto>>
                {
                    Success = true,

                    Message =
                        "Kullanıcılar başarıyla getirildi.",

                    Data =
                        users
                });
        }


        [HttpGet("{id:int:min(1)}")]
        public async Task<IActionResult>
            GetById(
                int id)
        {
            var user =
                await _userManagementService
                    .GetByIdAsync(id);

            if (user == null)
            {
                return NotFound(
                    Error(
                        "Kullanıcı bulunamadı."));
            }


            return Ok(
                new ApiResponse<UserDto>
                {
                    Success = true,

                    Message =
                        "Kullanıcı başarıyla getirildi.",

                    Data =
                        user
                });
        }


        [HttpPost]
        public async Task<IActionResult>
            CreateUser(
                CreateUserDto dto)
        {
            var result =
                await _userManagementService
                    .CreateAsync(dto);


            return result.Status switch
            {
                UserManagementOperationStatus
                    .UsernameAlreadyExists =>
                    Conflict(
                        Error(
                            "Bu kullanıcı adı daha önce alınmış.")),


                UserManagementOperationStatus
                    .InvalidRole =>
                    BadRequest(
                        Error(
                            "Geçersiz kullanıcı rolü.")),


                UserManagementOperationStatus
                    .EmployeeNotFound =>
                    BadRequest(
                        Error(
                            "Personel bulunamadı.")),


                UserManagementOperationStatus
                    .EmployeeInactive =>
                    BadRequest(
                        Error(
                            "Pasif personele kullanıcı hesabı bağlanamaz.")),


                UserManagementOperationStatus
                    .EmployeeAlreadyLinked =>
                    Conflict(
                        Error(
                            "Bu personel başka bir kullanıcı hesabına bağlı.")),


                _ =>
                    CreatedAtAction(
                        nameof(GetById),
                        new
                        {
                            id =
                                result.User!.Id
                        },
                        new ApiResponse<UserDto>
                        {
                            Success = true,

                            Message =
                                "Kullanıcı başarıyla oluşturuldu.",

                            Data =
                                result.User
                        })
            };
        }


        [HttpPut("{id:int:min(1)}/access")]
        public async Task<IActionResult>
            UpdateAccess(
                int id,
                UpdateUserAccessDto dto)
        {
            var result =
                await _userManagementService
                    .UpdateAccessAsync(
                        id,
                        dto);


            return result.Status switch
            {
                UserManagementOperationStatus
                    .UserNotFound =>
                    NotFound(
                        Error(
                            "Kullanıcı bulunamadı.")),


                UserManagementOperationStatus
                    .EmployeeNotFound =>
                    BadRequest(
                        Error(
                            "Personel bulunamadı.")),


                UserManagementOperationStatus
                    .EmployeeInactive =>
                    BadRequest(
                        Error(
                            "Pasif personele kullanıcı hesabı bağlanamaz.")),


                UserManagementOperationStatus
                    .EmployeeAlreadyLinked =>
                    Conflict(
                        Error(
                            "Bu personel başka bir kullanıcı hesabına bağlı.")),


                UserManagementOperationStatus
                    .InvalidRole =>
                    BadRequest(
                        Error(
                            "Geçersiz kullanıcı rolü.")),


                _ =>
                    Ok(
                        new ApiResponse<UserDto>
                        {
                            Success = true,

                            Message =
                                "Kullanıcı erişim bilgileri güncellendi.",

                            Data =
                                result.User
                        })
            };
        }


        [HttpPut("{id:int:min(1)}/status")]
        public async Task<IActionResult>
            UpdateStatus(
                int id,
                UpdateUserStatusDto dto)
        {
            var currentUserIdValue =
                User.FindFirst(
                    ClaimTypes.NameIdentifier)?
                    .Value;


            if (!int.TryParse(
                    currentUserIdValue,
                    out var currentUserId))
            {
                return Unauthorized(
                    Error(
                        "Kullanıcı kimliği doğrulanamadı."));
            }


            var result =
                await _userManagementService
                    .UpdateStatusAsync(
                        id,
                        currentUserId,
                        dto);


            return result.Status switch
            {
                UserManagementOperationStatus
                    .UserNotFound =>
                    NotFound(
                        Error(
                            "Kullanıcı bulunamadı.")),


                UserManagementOperationStatus
                    .CannotDeactivateSelf =>
                    BadRequest(
                        Error(
                            "Kendi yönetici hesabınızı pasifleştiremezsiniz.")),


                _ =>
                    Ok(
                        new ApiResponse<UserDto>
                        {
                            Success = true,

                            Message =
                                result.User!.IsActive
                                    ? "Kullanıcı hesabı aktifleştirildi."
                                    : "Kullanıcı hesabı pasifleştirildi.",

                            Data =
                                result.User
                        })
            };
        }


        private static ApiResponse<object?>
            Error(
                string message)
        {
            return new ApiResponse<object?>
            {
                Success = false,
                Message = message
            };
        }
    }
}