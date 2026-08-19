namespace Kipas.Personel.API.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Username { get; set; } =
            string.Empty;

        public string Role { get; set; } =
            string.Empty;

        public bool IsActive { get; set; }

        public int? EmployeeId { get; set; }

        public string? EmployeeName { get; set; }

        public string? DepartmentName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}