using System.ComponentModel.DataAnnotations;

namespace Kipas.Personel.API.DTOs
{
    public class UpdateUserAccessDto
    {
        [Required]
        [MaxLength(20)]
        public string Role { get; set; } =
            string.Empty;

        [Range(
            1,
            int.MaxValue,
            ErrorMessage =
                "Geçerli bir personel seçilmelidir.")]
        public int? EmployeeId { get; set; }
    }
}