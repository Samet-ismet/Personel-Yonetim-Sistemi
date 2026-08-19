namespace Kipas.Personel.API.DTOs
{
    public class EmployeeDetailDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; } =
            string.Empty;

        public string LastName { get; set; } =
            string.Empty;

        public string Email { get; set; } =
            string.Empty;

        public string PhoneNumber { get; set; } =
            string.Empty;

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } =
            string.Empty;

        public string Position { get; set; } =
            string.Empty;

        public DateTime HireDate { get; set; }

        public bool IsActive { get; set; }
    }
}