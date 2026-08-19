namespace Kipas.Personel.API.DTOs
{
    public class EmployeeCvDto
    {
        public int EmployeeId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; }
    }
}