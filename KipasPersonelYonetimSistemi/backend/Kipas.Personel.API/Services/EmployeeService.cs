using AutoMapper;
using Microsoft.AspNetCore.Http;
using Kipas.Personel.API.DTOs;
using Kipas.Personel.API.Entities;
using Kipas.Personel.API.Helpers;
using Kipas.Personel.API.Interfaces;

namespace Kipas.Personel.API.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IAppUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(
            IEmployeeRepository repository,
            IDepartmentRepository departmentRepository,
            IAppUserRepository userRepository,
            IMapper mapper,
            IFileStorageService fileStorageService,
            ILogger<EmployeeService> logger)
        {
            _repository = repository;
            _departmentRepository = departmentRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        public async Task<PagedResult<EmployeeDto>>
            GetAllAsync(
                EmployeeQueryParameters queryParameters)
        {
            var result =
                await _repository.GetPagedAsync(
                    queryParameters);

            return new PagedResult<EmployeeDto>
            {
                Items =
                    _mapper.Map<List<EmployeeDto>>(
                        result.Items),

                PageNumber =
                    queryParameters.PageNumber,

                PageSize =
                    queryParameters.PageSize,

                TotalCount =
                    result.TotalCount
            };
        }

        public async Task<PagedResult<EmployeeDto>?>
            GetManagerDepartmentAsync(
                int userId,
                EmployeeQueryParameters queryParameters)
        {
            var user =
                await _userRepository.GetByIdAsync(
                    userId);

            if (user?.Employee == null)
            {
                return null;
            }

            if (!user.Employee.IsActive)
            {
                return null;
            }

            queryParameters.DepartmentId =
                user.Employee.DepartmentId;

            return await GetAllAsync(
                queryParameters);
        }

        public async Task<EmployeeDetailDto?>
            GetByIdAsync(
                int id)
        {
            var employee =
                await _repository.GetByIdAsync(
                    id);

            if (employee == null)
            {
                return null;
            }

            return _mapper.Map<EmployeeDetailDto>(
                employee);
        }

        public async Task<EmployeeDetailDto?>
     GetCurrentUserEmployeeAsync(
         int userId)
        {
            var user =
                await _userRepository.GetByIdAsync(
                    userId);

            if (user?.Employee == null)
            {
                return null;
            }

            if (!user.Employee.IsActive)
            {
                return null;
            }

            return _mapper.Map<EmployeeDetailDto>(
            user.Employee);
        }

        public async Task<EmployeeOperationResult>
            AddAsync(
                CreateEmployeeDto dto)
        {
            var department =
                await _departmentRepository.GetByIdAsync(
                    dto.DepartmentId);

            if (department == null)
            {
                return new EmployeeOperationResult
                {
                    Status =
                        EmployeeOperationStatus
                            .DepartmentNotFound
                };
            }

            if (!department.IsActive)
            {
                return new EmployeeOperationResult
                {
                    Status =
                        EmployeeOperationStatus
                            .DepartmentInactive
                };
            }

            var employee =
                _mapper.Map<Employee>(dto);

            employee.DepartmentId =
                department.Id;

            employee.Department =
                department;

            await _repository.AddAsync(
                employee);

            await _repository.SaveChangesAsync();

            _logger.LogInformation(
                "Yeni personel eklendi. Personel Id: {EmployeeId}, Departman Id: {DepartmentId}",
                employee.Id,
                department.Id);

            return new EmployeeOperationResult
            {
                Status =
                    EmployeeOperationStatus.Success,

                Employee =
                    _mapper.Map<EmployeeDto>(
                        employee)
            };
        }

        public async Task<EmployeeOperationResult>
            UpdateAsync(
                int id,
                UpdateEmployeeDto dto)
        {
            var employee =
                await _repository.GetByIdAsync(
                    id);

            if (employee == null)
            {
                _logger.LogWarning(
                    "Güncellenecek personel bulunamadı. Personel Id: {EmployeeId}",
                    id);

                return new EmployeeOperationResult
                {
                    Status =
                        EmployeeOperationStatus
                            .EmployeeNotFound
                };
            }

            var department =
                await _departmentRepository.GetByIdAsync(
                    dto.DepartmentId);

            if (department == null)
            {
                return new EmployeeOperationResult
                {
                    Status =
                        EmployeeOperationStatus
                            .DepartmentNotFound
                };
            }

            if (!department.IsActive)
            {
                return new EmployeeOperationResult
                {
                    Status =
                        EmployeeOperationStatus
                            .DepartmentInactive
                };
            }

            _mapper.Map(
                dto,
                employee);

            employee.DepartmentId =
                department.Id;

            employee.Department =
                department;

            await _repository.UpdateAsync(
                employee);

            await _repository.SaveChangesAsync();

            _logger.LogInformation(
                "Personel güncellendi. Personel Id: {EmployeeId}, Departman Id: {DepartmentId}",
                employee.Id,
                department.Id);

            return new EmployeeOperationResult
            {
                Status =
                    EmployeeOperationStatus.Success,

                Employee =
                    _mapper.Map<EmployeeDto>(
                        employee)
            };
        }

        public async Task<EmployeeOperationResult>
    DeleteAsync(
        int id)
        {
            var employee =
                await _repository.GetByIdAsync(
                    id);

            if (employee == null)
            {
                _logger.LogWarning(
                    "Silinecek personel bulunamadı. Personel Id: {EmployeeId}",
                    id);

                return new EmployeeOperationResult
                {
                    Status =
                        EmployeeOperationStatus
                            .EmployeeNotFound
                };
            }


            var hasLinkedUser =
                await _repository.HasLinkedUserAsync(
                    id);

            if (hasLinkedUser)
            {
                _logger.LogWarning(
                    "Kullanıcı hesabına bağlı personel silme işlemi engellendi. Personel Id: {EmployeeId}",
                    id);

                return new EmployeeOperationResult
                {
                    Status =
                        EmployeeOperationStatus
                            .EmployeeLinkedToUser
                };
            }


            var storedCvFileName =
                employee.CvStoredFileName;


            await _repository.DeleteAsync(
                employee);

            await _repository.SaveChangesAsync();


            if (!string.IsNullOrWhiteSpace(
                    storedCvFileName))
            {
                try
                {
                    await _fileStorageService
                        .DeleteAsync(
                            storedCvFileName);
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(
                        exception,
                        "Personel silindi ancak CV dosyası diskten silinemedi. Dosya: {StoredFileName}",
                        storedCvFileName);
                }
            }


            _logger.LogInformation(
                "Personel silindi. Personel Id: {EmployeeId}",
                id);


            return new EmployeeOperationResult
            {
                Status =
                    EmployeeOperationStatus.Success
            };
        }

        public async Task<EmployeeCvDto?>
            UploadCvAsync(
                int employeeId,
                IFormFile file,
                CancellationToken cancellationToken)
        {
            var employee =
                await _repository.GetByIdAsync(
                    employeeId);

            if (employee == null)
            {
                return null;
            }

            var oldStoredFileName =
                employee.CvStoredFileName;

            var newStoredFileName =
                await _fileStorageService
                    .SavePdfAsync(
                        file,
                        cancellationToken);

            var originalFileName =
                Path.GetFileName(
                    file.FileName);

            if (string.IsNullOrWhiteSpace(
                    originalFileName))
            {
                originalFileName =
                    "cv.pdf";
            }

            var uploadedAt =
                DateTime.UtcNow;

            employee.CvOriginalFileName =
                originalFileName;

            employee.CvStoredFileName =
                newStoredFileName;

            employee.CvContentType =
                "application/pdf";

            employee.CvFileSize =
                file.Length;

            employee.CvUploadedAt =
                uploadedAt;

            try
            {
                await _repository.UpdateAsync(
                    employee);

                await _repository
                    .SaveChangesAsync();
            }
            catch
            {
                try
                {
                    await _fileStorageService
                        .DeleteAsync(
                            newStoredFileName);
                }
                catch (
                    Exception cleanupException)
                {
                    _logger.LogWarning(
                        cleanupException,
                        "Veritabanı güncellemesi başarısız olduktan sonra yeni CV dosyası temizlenemedi. Dosya: {StoredFileName}",
                        newStoredFileName);
                }

                throw;
            }

            if (!string.IsNullOrWhiteSpace(
                    oldStoredFileName))
            {
                try
                {
                    await _fileStorageService
                        .DeleteAsync(
                            oldStoredFileName);
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(
                        exception,
                        "Eski CV dosyası silinemedi. Dosya: {StoredFileName}",
                        oldStoredFileName);
                }
            }

            _logger.LogInformation(
                "Personel CV dosyası yüklendi. Personel Id: {EmployeeId}",
                employeeId);

            return new EmployeeCvDto
            {
                EmployeeId =
                    employee.Id,

                FileName =
                    originalFileName,

                FileSize =
                    file.Length,

                UploadedAt =
                    uploadedAt
            };
        }

        public async Task<EmployeeCvDownloadResult?>
            DownloadCvAsync(
                int employeeId)
        {
            var employee =
                await _repository.GetByIdAsync(
                    employeeId);

            if (employee == null ||
                string.IsNullOrWhiteSpace(
                    employee.CvStoredFileName))
            {
                return null;
            }

            var stream =
                _fileStorageService.OpenRead(
                    employee.CvStoredFileName);

            if (stream == null)
            {
                _logger.LogWarning(
                    "CV veritabanında kayıtlı ancak disk üzerinde bulunamadı. Personel Id: {EmployeeId}",
                    employeeId);

                return null;
            }

            return new EmployeeCvDownloadResult
            {
                FileStream =
                    stream,

                ContentType =
                    employee.CvContentType ??
                    "application/pdf",

                FileName =
                    employee.CvOriginalFileName ??
                    "cv.pdf"
            };
        }

        public async Task<bool>
            DeleteCvAsync(
                int employeeId)
        {
            var employee =
                await _repository.GetByIdAsync(
                    employeeId);

            if (employee == null ||
                string.IsNullOrWhiteSpace(
                    employee.CvStoredFileName))
            {
                return false;
            }

            var storedFileName =
                employee.CvStoredFileName;

            employee.CvOriginalFileName =
                null;

            employee.CvStoredFileName =
                null;

            employee.CvContentType =
                null;

            employee.CvFileSize =
                null;

            employee.CvUploadedAt =
                null;

            await _repository.UpdateAsync(
                employee);

            await _repository.SaveChangesAsync();

            try
            {
                await _fileStorageService
                    .DeleteAsync(
                        storedFileName);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(
                    exception,
                    "CV veritabanından kaldırıldı ancak disk dosyası silinemedi. Dosya: {StoredFileName}",
                    storedFileName);
            }

            _logger.LogInformation(
                "Personel CV dosyası silindi. Personel Id: {EmployeeId}",
                employeeId);

            return true;
        }
    }
}