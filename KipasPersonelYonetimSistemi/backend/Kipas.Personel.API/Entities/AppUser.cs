using System.ComponentModel.DataAnnotations;

namespace Kipas.Personel.API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } =
            string.Empty;

        [Required]
        [MaxLength(256)]
        public string PasswordHash { get; set; } =
            string.Empty;

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "Employee";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } =
            DateTime.UtcNow;

        public int? EmployeeId { get; set; }

        public Employee? Employee { get; set; }

        public ICollection<RefreshToken>
            RefreshTokens
        { get; set; }
                = new List<RefreshToken>();
    }
}