using System.ComponentModel.DataAnnotations;

namespace Kipas.Personel.API.DTOs
{
    public class CreateDepartmentDto
    {
        [Required(
            ErrorMessage = "Departman adı zorunludur.")]
        [MaxLength(
            100,
            ErrorMessage =
                "Departman adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; } =
            string.Empty;

        [MaxLength(
            250,
            ErrorMessage =
                "Açıklama en fazla 250 karakter olabilir.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } =
            true;
    }
}