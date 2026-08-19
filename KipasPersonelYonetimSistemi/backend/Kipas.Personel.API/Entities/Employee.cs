using System.ComponentModel.DataAnnotations;

namespace Kipas.Personel.API.Entities;

public class Employee
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(254)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;

    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;

    [MaxLength(100)]
    public string Position { get; set; } = string.Empty;

    public DateTime HireDate { get; set; }

    public bool IsActive { get; set; }

    [MaxLength(255)]
    public string? CvOriginalFileName { get; set; }

    [MaxLength(100)]
    public string? CvStoredFileName { get; set; }

    [MaxLength(100)]
    public string? CvContentType { get; set; }

    public long? CvFileSize { get; set; }

    public DateTime? CvUploadedAt { get; set; }

    public AppUser? AppUser { get; set; }
}