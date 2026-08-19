namespace Kipas.Personel.API.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; } =
            string.Empty;

        public string LastName { get; set; } =
            string.Empty;

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } =
            string.Empty;

        public bool IsActive { get; set; }
    }
}