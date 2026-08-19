using System.ComponentModel.DataAnnotations;

namespace Kipas.Personel.API.Entities
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } =
            string.Empty;

        [MaxLength(250)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } =
            true;

        public DateTime CreatedAt { get; set; } =
            DateTime.UtcNow;

        public ICollection<Employee> Employees { get; set; }
= new List<Employee>();
    }
}