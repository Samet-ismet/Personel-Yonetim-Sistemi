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
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeeController(
            IEmployeeService service)
        {
            _service = service;
        }


        [HttpGet]
        [Authorize(
            Roles = RoleNames.AdminOrHumanResources)]
        public async Task<IActionResult> GetEmployees(
            [FromQuery]
            EmployeeQueryParameters queryParameters)
        {
            var result =
                await _service.GetAllAsync(
                    queryParameters);

            return Ok(
                new ApiResponse<
                    PagedResult<EmployeeDto>>
                {
                    Success = true,
                    Message =
                        "Personel listesi başarıyla getirildi.",
                    Data = result
                });
        }


        [HttpGet("my-department")]
        [Authorize(Roles = RoleNames.Manager)]
        public async Task<IActionResult>
            GetMyDepartmentEmployees(
                [FromQuery]
                EmployeeQueryParameters queryParameters)
        {
            var userIdValue =
                User.FindFirst(
                    ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(
                    userIdValue,
                    out var userId))
            {
                return Unauthorized(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            "Kullanıcı kimliği doğrulanamadı."
                    });
            }

            var result =
                await _service
                    .GetManagerDepartmentAsync(
                        userId,
                        queryParameters);

            if (result == null)
            {
                return Forbid();
            }

            return Ok(
                new ApiResponse<
                    PagedResult<EmployeeDto>>
                {
                    Success = true,
                    Message =
                        "Departman personelleri başarıyla getirildi.",
                    Data = result
                });
        }


        [HttpGet("me")]
        public async Task<IActionResult>
            GetMyEmployeeProfile()
        {
            var userIdValue =
                User.FindFirst(
                    ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(
                    userIdValue,
                    out var userId))
            {
                return Unauthorized(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            "Kullanıcı kimliği doğrulanamadı."
                    });
            }

            var employee =
                await _service
                    .GetCurrentUserEmployeeAsync(
                        userId);

            if (employee == null)
            {
                return NotFound(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            "Kullanıcı hesabına bağlı aktif personel kaydı bulunamadı."
                    });
            }

            return Ok(
              new ApiResponse<EmployeeDetailDto>
              {
                    Success = true,
                    Message =
                        "Personel profili başarıyla getirildi.",
                    Data = employee
                });
        }


        [HttpGet("{id:int:min(1)}")]
        [Authorize(
            Roles = RoleNames.AdminOrHumanResources)]
        public async Task<IActionResult>
            GetEmployeeById(
                int id)
        {
            var employee =
                await _service.GetByIdAsync(
                    id);

            if (employee == null)
            {
                return NotFound(
                    new ApiResponse<
                        EmployeeDetailDto>
                    {
                        Success = false,
                        Message =
                            "Personel bulunamadı."
                    });
            }

            return Ok(
                new ApiResponse<
                    EmployeeDetailDto>
                {
                    Success = true,
                    Message =
                        "Personel başarıyla getirildi.",
                    Data = employee
                });
        }


        [HttpPost]
        [Authorize(
            Roles = RoleNames.AdminOrHumanResources)]
        public async Task<IActionResult> AddEmployee(
            CreateEmployeeDto dto)
        {
            var result =
                await _service.AddAsync(
                    dto);

            if (result.Status ==
                EmployeeOperationStatus
                    .DepartmentNotFound)
            {
                return BadRequest(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            "Seçilen departman bulunamadı."
                    });
            }

            if (result.Status ==
                EmployeeOperationStatus
                    .DepartmentInactive)
            {
                return BadRequest(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            "Pasif bir departmana personel atanamaz."
                    });
            }

            return CreatedAtAction(
                nameof(GetEmployeeById),
                new
                {
                    id = result.Employee!.Id
                },
                new ApiResponse<EmployeeDto>
                {
                    Success = true,
                    Message =
                        "Personel başarıyla eklendi.",
                    Data = result.Employee
                });
        }


        [HttpPut("{id:int:min(1)}")]
        [Authorize(
            Roles = RoleNames.AdminOrHumanResources)]
        public async Task<IActionResult>
            UpdateEmployee(
                int id,
                UpdateEmployeeDto dto)
        {
            var result =
                await _service.UpdateAsync(
                    id,
                    dto);

            if (result.Status ==
                EmployeeOperationStatus
                    .EmployeeNotFound)
            {
                return NotFound(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            "Güncellenecek personel bulunamadı."
                    });
            }

            if (result.Status ==
                EmployeeOperationStatus
                    .DepartmentNotFound)
            {
                return BadRequest(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            "Seçilen departman bulunamadı."
                    });
            }

            if (result.Status ==
                EmployeeOperationStatus
                    .DepartmentInactive)
            {
                return BadRequest(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            "Personel pasif bir departmana atanamaz."
                    });
            }

            return Ok(
                new ApiResponse<EmployeeDto>
                {
                    Success = true,
                    Message =
                        "Personel başarıyla güncellendi.",
                    Data = result.Employee
                });
        }


        [HttpDelete("{id:int:min(1)}")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult>
    DeleteEmployee(
        int id)
        {
            var result =
                await _service.DeleteAsync(
                    id);


            if (result.Status ==
                EmployeeOperationStatus
                    .EmployeeNotFound)
            {
                return NotFound(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            "Silinecek personel bulunamadı."
                    });
            }


            if (result.Status ==
                EmployeeOperationStatus
                    .EmployeeLinkedToUser)
            {
                return Conflict(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            "Bu personel bir kullanıcı hesabına bağlı olduğu için silinemez. Önce Kullanıcı Yönetimi ekranından personel bağlantısını kaldırınız."
                    });
            }


            return Ok(
                new ApiResponse<object?>
                {
                    Success = true,
                    Message =
                        "Personel başarıyla silindi."
                });
        }

        [HttpPost("{id:int:min(1)}/cv")]
        [Authorize(
            Roles = RoleNames.AdminOrHumanResources)]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(
            6 * 1024 * 1024)]
        public async Task<IActionResult> UploadCv(
            int id,
            [FromForm] UploadEmployeeCvDto dto,
            CancellationToken cancellationToken)
        {
            var validationError =
                await PdfFileValidator
                    .ValidateAsync(
                        dto.File,
                        cancellationToken);

            if (validationError != null)
            {
                return BadRequest(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            validationError
                    });
            }

            var result =
                await _service.UploadCvAsync(
                    id,
                    dto.File,
                    cancellationToken);

            if (result == null)
            {
                return NotFound(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            "CV yüklenecek personel bulunamadı."
                    });
            }

            return Ok(
                new ApiResponse<EmployeeCvDto>
                {
                    Success = true,
                    Message =
                        "Personel CV dosyası başarıyla yüklendi.",
                    Data = result
                });
        }


        [HttpGet("{id:int:min(1)}/cv")]
        [Authorize(
            Roles = RoleNames.AdminOrHumanResources)]
        public async Task<IActionResult>
            DownloadCv(
                int id)
        {
            var result =
                await _service.DownloadCvAsync(
                    id);

            if (result == null)
            {
                return NotFound(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            "Personele ait CV dosyası bulunamadı."
                    });
            }

            return File(
                result.FileStream,
                result.ContentType,
                result.FileName,
                enableRangeProcessing: true);
        }


        [HttpDelete("{id:int:min(1)}/cv")]
        [Authorize(
            Roles = RoleNames.AdminOrHumanResources)]
        public async Task<IActionResult>
            DeleteCv(
                int id)
        {
            var result =
                await _service.DeleteCvAsync(
                    id);

            if (!result)
            {
                return NotFound(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            "Silinecek CV dosyası bulunamadı."
                    });
            }

            return Ok(
                new ApiResponse<object?>
                {
                    Success = true,
                    Message =
                        "Personel CV dosyası başarıyla silindi."
                });
        }
    }
}