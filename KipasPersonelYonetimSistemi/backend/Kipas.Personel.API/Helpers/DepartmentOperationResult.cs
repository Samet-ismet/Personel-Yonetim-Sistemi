using Kipas.Personel.API.DTOs;

namespace Kipas.Personel.API.Helpers
{
    public sealed class DepartmentOperationResult
    {
        public DepartmentOperationStatus Status
        {
            get;
            init;
        }

        public DepartmentDto? Department
        {
            get;
            init;
        }
    }
}