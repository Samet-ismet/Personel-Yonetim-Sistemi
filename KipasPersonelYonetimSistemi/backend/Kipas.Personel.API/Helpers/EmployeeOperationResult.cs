using Kipas.Personel.API.DTOs;

namespace Kipas.Personel.API.Helpers
{
    public sealed class EmployeeOperationResult
    {
        public EmployeeOperationStatus Status { get; init; }

        public EmployeeDto? Employee { get; init; }
    }
}