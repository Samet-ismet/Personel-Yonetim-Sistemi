using AutoMapper;
using Kipas.Personel.API.DTOs;
using Kipas.Personel.API.Entities;
using Kipas.Personel.API.Helpers;
using Kipas.Personel.API.Interfaces;

namespace Kipas.Personel.API.Services
{
    public class DepartmentService
        : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<DepartmentService> _logger;

        public DepartmentService(
            IDepartmentRepository repository,
            IMapper mapper,
            ILogger<DepartmentService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<DepartmentDto>>
            GetAllAsync(bool includeInactive)
        {
            var departments =
                await _repository.GetAllAsync(
                    includeInactive);

            return _mapper.Map<List<DepartmentDto>>(
                departments);
        }

        public async Task<DepartmentDto?> GetByIdAsync(
            int id)
        {
            var department =
                await _repository.GetByIdAsync(id);

            return department == null
                ? null
                : _mapper.Map<DepartmentDto>(department);
        }

        public async Task<DepartmentOperationResult>
            AddAsync(CreateDepartmentDto dto)
        {
            var name = dto.Name.Trim();

            var nameExists =
                await _repository.NameExistsAsync(name);

            if (nameExists)
            {
                return new DepartmentOperationResult
                {
                    Status =
                        DepartmentOperationStatus
                            .DuplicateName
                };
            }

            var department = new Department
            {
                Name = name,
                Description =
                    NormalizeDescription(dto.Description),
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(department);
            await _repository.SaveChangesAsync();

            _logger.LogInformation(
                "Departman oluşturuldu. Departman Id: {DepartmentId}",
                department.Id);

            return new DepartmentOperationResult
            {
                Status = DepartmentOperationStatus.Success,
                Department =
                    _mapper.Map<DepartmentDto>(department)
            };
        }

        public async Task<DepartmentOperationResult>
            UpdateAsync(
                int id,
                UpdateDepartmentDto dto)
        {
            var department =
                await _repository.GetByIdAsync(id);

            if (department == null)
            {
                return new DepartmentOperationResult
                {
                    Status =
                        DepartmentOperationStatus.NotFound
                };
            }

            var name = dto.Name.Trim();

            var nameExists =
                await _repository.NameExistsAsync(
                    name,
                    id);

            if (nameExists)
            {
                return new DepartmentOperationResult
                {
                    Status =
                        DepartmentOperationStatus
                            .DuplicateName
                };
            }

            department.Name = name;
            department.Description =
                NormalizeDescription(dto.Description);
            department.IsActive = dto.IsActive;

            await _repository.SaveChangesAsync();

            _logger.LogInformation(
                "Departman güncellendi. Departman Id: {DepartmentId}",
                department.Id);

            return new DepartmentOperationResult
            {
                Status = DepartmentOperationStatus.Success,
                Department =
                    _mapper.Map<DepartmentDto>(department)
            };
        }

        public async Task<DepartmentOperationStatus>
            DeactivateAsync(int id)
        {
            var department =
                await _repository.GetByIdAsync(id);

            if (department == null)
            {
                return DepartmentOperationStatus.NotFound;
            }

            if (!department.IsActive)
            {
                return DepartmentOperationStatus.Success;
            }

            department.IsActive = false;

            await _repository.SaveChangesAsync();

            _logger.LogInformation(
                "Departman pasif hâle getirildi. Departman Id: {DepartmentId}",
                department.Id);

            return DepartmentOperationStatus.Success;
        }

        private static string? NormalizeDescription(
            string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return null;
            }

            return description.Trim();
        }
    }
}