using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kipas.Personel.API.DTOs;
using Kipas.Personel.API.Helpers;
using Kipas.Personel.API.Interfaces;

namespace Kipas.Personel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;

        public DepartmentController(
            IDepartmentService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(
            Roles = RoleNames.AdminOrHumanResources)]
        public async Task<IActionResult> GetDepartments(
            [FromQuery] bool includeInactive = false)
        {
            var departments =
                await _service.GetAllAsync(
                    includeInactive);

            return Ok(
                new ApiResponse<List<DepartmentDto>>
                {
                    Success = true,
                    Message =
                        "Departman listesi başarıyla getirildi.",
                    Data = departments
                });
        }

        [HttpGet("{id:int:min(1)}")]
        [Authorize(
            Roles = RoleNames.AdminOrHumanResources)]
        public async Task<IActionResult> GetDepartmentById(
            int id)
        {
            var department =
                await _service.GetByIdAsync(id);

            if (department == null)
            {
                return NotFound(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            "Departman bulunamadı."
                    });
            }

            return Ok(
                new ApiResponse<DepartmentDto>
                {
                    Success = true,
                    Message =
                        "Departman başarıyla getirildi.",
                    Data = department
                });
        }

        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> AddDepartment(
            CreateDepartmentDto dto)
        {
            var result =
                await _service.AddAsync(dto);

            if (result.Status ==
                DepartmentOperationStatus.DuplicateName)
            {
                return Conflict(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            "Bu isimde bir departman zaten mevcut."
                    });
            }

            return CreatedAtAction(
                nameof(GetDepartmentById),
                new
                {
                    id = result.Department!.Id
                },
                new ApiResponse<DepartmentDto>
                {
                    Success = true,
                    Message =
                        "Departman başarıyla oluşturuldu.",
                    Data = result.Department
                });
        }

        [HttpPut("{id:int:min(1)}")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> UpdateDepartment(
            int id,
            UpdateDepartmentDto dto)
        {
            var result =
                await _service.UpdateAsync(
                    id,
                    dto);

            if (result.Status ==
                DepartmentOperationStatus.NotFound)
            {
                return NotFound(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            "Güncellenecek departman bulunamadı."
                    });
            }

            if (result.Status ==
                DepartmentOperationStatus.DuplicateName)
            {
                return Conflict(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            "Bu isimde bir departman zaten mevcut."
                    });
            }

            return Ok(
                new ApiResponse<DepartmentDto>
                {
                    Success = true,
                    Message =
                        "Departman başarıyla güncellendi.",
                    Data = result.Department
                });
        }

        [HttpDelete("{id:int:min(1)}")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> DeactivateDepartment(
            int id)
        {
            var status =
                await _service.DeactivateAsync(id);

            if (status ==
                DepartmentOperationStatus.NotFound)
            {
                return NotFound(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            "Pasif hâle getirilecek departman bulunamadı."
                    });
            }

            return Ok(
                new ApiResponse<object?>
                {
                    Success = true,
                    Message =
                        "Departman başarıyla pasif hâle getirildi."
                });
        }
    }
}